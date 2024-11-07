using Microsoft.AspNetCore.Http;

namespace CoarUtils.commands.web {
  public class IsIosDevice {
    public static bool Execute(HttpRequest hr) {
      var userAgent = hr.Headers["User-Agent"].ToString().ToLower();
      return (
        userAgent.Contains("iphone;")
        ||
        userAgent.Contains("ipad;")
        ||
        userAgent.Contains("ipod")
      );
    }
  }
}
