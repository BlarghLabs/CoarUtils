using CoarUtils.commands.logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace CoarUtils.commands.web {
  public class GetDisplayUrl {
    public static string Execute(
      HttpContext hc
    ) {
      //HttpContext.Current.Request.Url.AbsolutePath
      if (
        (hc == null)
        ||
        (hc.Request == null)
      ) {
        return null;
      }
      try {
        return hc.Request.GetDisplayUrl();
      } catch (Exception ex) {
        //until I know the behavior
        LogIt.I(ex, CancellationToken.None);
        return null;
      }
    }
  }
}
