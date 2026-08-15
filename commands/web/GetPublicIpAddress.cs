using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.web {
  public class GetPublicIpAddress {
    /// <summary>
    /// account for possbility of ELB sheilding the public ip
    /// </summary>
    /// <returns></returns>
    public static string Execute(
      HttpContext hc,
      bool log = false
    ) {
      var remoteIpAddress = "";
      var xForwardedFor = "";
      var remoteAddr = "";
      var ip = "";
      var status = "";
      try {
        //https://stackoverflow.com/questions/38571032/how-to-get-httpcontext-current-in-asp-net-core
        //TODO: get if not passed

        if ((hc == null) || (hc.Request == null)) {
          status = "http context was null";
          return null;
        }

        remoteIpAddress = hc.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
        if (!string.IsNullOrEmpty(hc.Request.Headers["X-Forwarded-For"])) {
          xForwardedFor = hc.Request.Headers["X-Forwarded-For"];
        }
        if (!string.IsNullOrEmpty(hc.Request.Headers["REMOTE_ADDR"])) {
          remoteAddr = hc.Request.Headers["REMOTE_ADDR"];
        }

        // X-Forwarded-For and REMOTE_ADDR are CLIENT-SETTABLE. Whatever a caller puts there arrives here
        // verbatim, and this value is persisted (user_activity.ip_address, external_api_usage_log.ip_address,
        // every *_status.ip_address) and used for IP-based support views and rate limiting.
        //
        // Two things were wrong before, both exploited by the 2026-08-14 sweep, whose SQL payloads
        // (`-1 OR 5*5=25 --`, `if(now()=sysdate()...`) are sitting in those columns as a result:
        //
        //   1. The value was never validated as an IP, so arbitrary text was stored.
        //   2. It took the FIRST entry of the X-Forwarded-For chain. Our ALB APPENDS the real client IP to
        //      whatever the client sent, so the chain is `<client-supplied>, <real client ip>` — the first
        //      entry is precisely the attacker-controlled half and the last is the one our own proxy wrote.
        //
        // So: walk the chain right-to-left and take the last entry that actually parses as an IP address. For
        // ordinary traffic the client sends no X-Forwarded-For, the chain has exactly one entry, and first ==
        // last — this changes nothing. It only changes the answer when a client supplied its own value, which
        // is exactly the case where the old answer was untrustworthy.
        ip = FirstValidIpAddressFromRightmost(xForwardedFor)
          ?? FirstValidIpAddressFromRightmost(remoteAddr)
          ?? (IsValidIpAddress(remoteIpAddress) ? remoteIpAddress : null);

        return ip;
      } catch (Exception ex) {
        LogIt.I(ex, CancellationToken.None);
      } finally {
        if (log) {
          LogIt.D(JsonConvert.SerializeObject(new {
            ip,
            status,
            remoteIpAddress,
            xForwardedFor,
            remoteAddr
          }, Formatting.Indented), CancellationToken.None);
        }
      }
      return null;
    }

    /// <summary>
    /// The raw forwarding headers the caller sent, as compact JSON, or null when it sent none.
    ///
    /// Execute() above deliberately discards the caller-supplied half of X-Forwarded-For, which is the right
    /// thing for attribution and the wrong thing for detection: it also discards the evidence that somebody
    /// tried to spoof. This is that evidence, stored alongside the resolved address.
    ///
    /// ip_address is the value we BELIEVE. This is what the caller CLAIMED. Read together:
    ///   * more than one X-Forwarded-For entry -> the caller supplied its own (our ALB appends exactly one).
    ///     Suspicious, but a corporate proxy does it innocently.
    ///   * an entry that cannot be an IP address -> forged. No legitimate client does this.
    ///
    /// The extra headers are ones we do NOT consume: anything arriving in them is a caller probing for a proxy
    /// layer that would honour them. Recorded so that probing is visible too.
    ///
    /// Never returns the connection's real address - only what the client sent, so the column is unambiguously
    /// "untrusted input" and can never be mistaken for a resolved value.
    /// </summary>
    public static string GetForwardedHeadersJson(HttpContext hc, int maxLength = 1024) {
      try {
        if ((hc == null) || (hc.Request == null)) {
          return null;
        }
        var captured = new Dictionary<string, string>();
        foreach (var headerName in new[] {
          "X-Forwarded-For", "REMOTE_ADDR", "X-Real-IP", "True-Client-IP", "CF-Connecting-IP",
          "X-Client-IP", "X-Cluster-Client-IP", "X-Originating-IP", "Forwarded", "Via",
        }) {
          var value = hc.Request.Headers[headerName].ToString();
          if (!string.IsNullOrWhiteSpace(value)) {
            captured[headerName] = value;
          }
        }
        if (captured.Count == 0) {
          return null;
        }

        // The verdict is decided HERE, where the headers can be parsed properly, and stored in the JSON — not
        // re-derived later in SQL. Downstream detection then does an exact string match on the verdict instead
        // of trying to guess intent out of a header value with LIKE patterns, which is both fragile and easy to
        // get subtly wrong. Only recorded when there is something to say, so its presence is itself the signal.
        var verdict = GetForwardedHeaderSpoofVerdict(hc.Request.Headers["X-Forwarded-For"].ToString());
        if (verdict != ForwardedHeaderSpoofVerdict.none) {
          captured["spoof"] = verdict.GetLabel();
        }

        var json = JsonConvert.SerializeObject(captured, Formatting.None);
        // Truncate rather than drop: a chain long enough to overflow the column is itself worth seeing, and
        // losing the row entirely would be the worse outcome.
        return json.Length > maxLength ? json.Substring(0, maxLength) : json;
      } catch (Exception ex) {
        // Never let evidence capture break the request it is describing.
        LogIt.I(ex, CancellationToken.None);
        return null;
      }
    }

    /// <summary>
    /// Classifies an X-Forwarded-For chain — see ForwardedHeaderSpoofVerdict for what each value means.
    /// Public and side-effect free so it can be unit tested directly without an HttpContext.
    /// </summary>
    public static ForwardedHeaderSpoofVerdict GetForwardedHeaderSpoofVerdict(string forwardedForChain) {
      if (string.IsNullOrWhiteSpace(forwardedForChain)) {
        return ForwardedHeaderSpoofVerdict.none;
      }
      var entries = forwardedForChain
        .Split(',')
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToList();
      if (entries.Count == 0) {
        return ForwardedHeaderSpoofVerdict.none;
      }
      // An unparseable entry anywhere in the chain is forged — it outranks the count, because a caller that
      // sent garbage has already told us what it is regardless of how many entries there are.
      if (entries.Any(x => !IsValidIpAddress(x))) {
        return ForwardedHeaderSpoofVerdict.forged;
      }
      // All entries parse. More than one means the caller supplied its own on top of our proxy's.
      return entries.Count > 1
        ? ForwardedHeaderSpoofVerdict.clientSupplied
        : ForwardedHeaderSpoofVerdict.none;
    }

    /// <summary>
    /// Last entry of a comma-separated forwarded-for chain that parses as an IP address, or null.
    /// Right-to-left because our proxy appends: the rightmost entry is the one it wrote, everything to the
    /// left of it was supplied by the caller and can say anything.
    /// </summary>
    public static string FirstValidIpAddressFromRightmost(string forwardedForChain) {
      if (string.IsNullOrWhiteSpace(forwardedForChain)) {
        return null;
      }
      var entries = forwardedForChain.Split(',');
      for (var i = entries.Length - 1; i >= 0; i--) {
        var candidate = entries[i].Trim();
        if (IsValidIpAddress(candidate)) {
          return candidate;
        }
      }
      return null;
    }

    /// <summary>
    /// True when the value is a parseable IPv4/IPv6 address. Strips an optional :port (IPv4 and bracketed
    /// IPv6 only — a bare IPv6 address is full of colons and must not be split on them).
    /// </summary>
    public static bool IsValidIpAddress(string value) {
      if (string.IsNullOrWhiteSpace(value)) {
        return false;
      }
      var candidate = value.Trim();
      if (candidate.StartsWith("[")) {
        var closingBracket = candidate.IndexOf(']');
        if (closingBracket > 0) {
          candidate = candidate.Substring(1, closingBracket - 1);
        }
      } else if (candidate.Count(x => x == ':') == 1) {
        candidate = candidate.Split(':').First();
      }
      return IPAddress.TryParse(candidate, out _);
    }
  }
}
