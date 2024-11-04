using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;
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
        LogIt.E(ex);
        return null;
      }
    }
  }
}
