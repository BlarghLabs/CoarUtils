using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CoarUtils.commands.web {
  public class GetUserAgent {
    /// <summary>
    /// account for possbility of ELB sheilding the user agent
    /// </summary>
    /// <returns></returns>
    public static string Execute(
      HttpContext context = null
    ) {
      var headerVersion = "";
      var userAgent  = "";
      //try {
      //  //https://stackoverflow.com/questions/38571032/how-to-get-httpcontext-current-in-asp-net-core
      //  //TODO: get if not passed

      //  if ((context == null) || (context.Request == null)) {
      //    return null;
      //  }

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
      //  LogIt.E(ex.Message);
      //} finally {
      //  LogIt.D(JsonConvert.SerializeObject(new {
      //    ip,
      //    remoteIpAddress,
      //    xForwardedFor,
      //    remoteAddr
      //  }, Formatting.Indented));
      //}
      return null;
    }
  }
}
