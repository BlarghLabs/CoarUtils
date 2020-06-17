using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace CoarUtils.commands.web {
  public class UnauthenticatedUser {
    public static bool Execute(
      IPrincipal ip
    ) {
      if (
        (ip == null)
        ||
        (ip.Identity == null)
        ||
        !ip.Identity.IsAuthenticated
      ) {
        return true;
      }

      var cp = (ClaimsPrincipal)ip;
      //note: i have seen mult name id and no name
      var nameId = cp.Claims
        .Where(x => x.Type.Equals(ClaimTypes.NameIdentifier))
        .Select(x=>x.Value)
        .FirstOrDefault()
      ;
      var name = cp.Claims
        .Where(x => x.Type.Equals(ClaimTypes.Name))
        .Select(x => x.Value)
        .FirstOrDefault()
      ;
      //////{http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: foo@bar.baz}
      if (
        string.IsNullOrEmpty(ip.Identity.Name)
        &&
        string.IsNullOrEmpty(name)
        &&
        string.IsNullOrEmpty(nameId)
      ) { 
        return true;
      }
      return false;
    }

  }
}
