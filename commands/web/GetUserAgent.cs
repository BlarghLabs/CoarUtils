using Microsoft.AspNetCore.Http;

namespace CoarUtils.commands.web {
  public class GetUserAgent {
    /// <summary>
    /// account for possbility of ELB sheilding the user agent
    /// </summary>
    /// <returns></returns>
    public static string Execute(
      HttpContext hc
    ) {
      if (
        (hc == null)
        ||
        (hc.Request == null)
        ||
        (hc.Request.Headers == null)
        ) {
        return null;
      }
      return hc.Request.Headers["User-Agent"];


      //  remoteIpAddress = context.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
      //  if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"])) {
      //    xForwardedFor = context.Request.Headers["X-Forwarded-For"];
      //  }
      //  if (!string.IsNullOrEmpty(context.Request.Headers["REMOTE_ADDR"])) {
      //    remoteAddr = context.Request.Headers["REMOTE_ADDR"];
      //  }

      //  var loIp = new List<string> { xForwardedFor, remoteAddr, remoteIpAddress };
      //  loIp = loIp.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
      //  ip = loIp.FirstOrDefault();
      //  if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(",")) {
      //    ip = ip.Split(",").First();
      //  }

      //  return ip;
      //} catch (Exception ex) {
      //  LogIt.I(ex,cancellationToken);
      //} finally {
      //  LogIt.D(JsonConvert.SerializeObject(new {
      //    ip,
      //    remoteIpAddress,
      //    xForwardedFor,
      //    remoteAddr
      //  }, Formatting.Indented), cancellationToken);
      //}
    }
  }
}
