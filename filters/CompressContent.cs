using Microsoft.AspNetCore.Mvc.Filters;

namespace CoarUtils.filters {
  /// <summary>
  /// Note: In ASP.NET Core, prefer using app.UseResponseCompression() middleware instead.
  /// Response.Filter is not available in ASP.NET Core.
  /// This is kept as a no-op for backward compatibility during migration.
  /// </summary>
  public class CompressContentAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context) {
      // ASP.NET Core handles compression via UseResponseCompression() middleware.
      base.OnActionExecuting(context);
    }
  }
}
