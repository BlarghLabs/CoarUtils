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
