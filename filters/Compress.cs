using Microsoft.AspNetCore.Mvc.Filters;

namespace CoarUtils.filters {
  /// <summary>
  /// Note: In ASP.NET Core, prefer using app.UseResponseCompression() middleware instead of this filter.
  /// This is kept as a no-op for backward compatibility during migration.
  /// </summary>
  public class Compress : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context) {
      // ASP.NET Core handles compression via UseResponseCompression() middleware.
      // Response.Filter is not available in ASP.NET Core.
      base.OnActionExecuting(context);
    }
  }
}
