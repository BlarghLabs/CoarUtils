using CoarUtils.commands.logging;
using System.Security.Principal;

namespace CoarUtils.commands.web {
  public class GetExecutingUsername {
    public static string Execute(
      IPrincipal ip = null
    ) {
      string executedBy = null;
      try {
        if (ip == null) {
          ip = Thread.CurrentPrincipal;
        }
        executedBy = (ip?.Identity == null || !ip.Identity.IsAuthenticated)
          ? null
          : ip.Identity.Name;
      } catch (Exception ex) {
        LogIt.E(ex);
      }
      return executedBy;
    }
  }


}