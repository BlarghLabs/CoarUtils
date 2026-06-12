using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CoarUtils.commands.web {
  public class MvcUtils {
    /// <summary>
    /// Get controller name from the current request's route data.
    /// Pass HttpContext from controller or IHttpContextAccessor.
    /// </summary>
    public static string GetControllerName(HttpContext hc) {
      if (hc?.Request == null) return "";
      var routeData = hc.GetRouteData();
      if (routeData == null) return "";
      routeData.Values.TryGetValue("controller", out var controller);
      return controller?.ToString()?.ToLower() ?? "";
    }

    /// <summary>
    /// Get action name from the current request's route data.
    /// Pass HttpContext from controller or IHttpContextAccessor.
    /// </summary>
    public static string GetActionName(HttpContext hc) {
      if (hc?.Request == null) return "";
      var routeData = hc.GetRouteData();
      if (routeData == null) return "";
      routeData.Values.TryGetValue("action", out var action);
      return action?.ToString()?.ToLower() ?? "";
    }

    public static bool IsMatch(HttpContext hc, string controller, string action) {
      return (GetControllerName(hc) == controller) && (GetActionName(hc) == action);
    }
  }
}
