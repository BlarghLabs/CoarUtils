using CoarUtils.commands.logging;
using System;
using System.Security.Principal;
using System.Threading;

namespace CoarUtils.commands.web {
  public class GetExecutingUsername {
    public static string Execute(
      IPrincipal ip
    ) {
      string executedBy = null;
      try {
        if (ip == null) {
          ip = Thread.CurrentPrincipal;
        }
        executedBy = (ip == null || ip.Identity == null || !ip.Identity.IsAuthenticated)
          ? null
          : ip.Identity.Name;
      } catch (Exception ex) {
        LogIt.E(ex);
      }
      return executedBy;
    }
  }


}