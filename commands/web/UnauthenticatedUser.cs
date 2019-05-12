using System.Security.Principal;

namespace CoarUtils.commands.web {
  public class UnauthenticatedUser {
    public static bool Execute(
      IPrincipal ip
    ) {
      //ip?.Identity?.Name ?? ""
      var result =
        (ip == null)
        || (ip.Identity == null)
        || !ip.Identity.IsAuthenticated
        || string.IsNullOrEmpty(ip.Identity.Name);
      return result;
    }

  }
}
