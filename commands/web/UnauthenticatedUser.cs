using System.Security.Claims;
using System.Security.Principal;

namespace CoarUtils.commands.web {
  public static class UnauthenticatedUser {

    public static bool IsUnauthenticated(
      this IPrincipal iPrincipal
    ) {
      return Execute(iPrincipal);
    }

    public static bool Execute(
      IPrincipal iPrincipal
    ) {
      if (
        (iPrincipal?.Identity == null)
        ||
        !iPrincipal.Identity.IsAuthenticated
      ) {
        return true;
      }

      var claimsPrincipal = (ClaimsPrincipal)iPrincipal;
      //note: i have seen mult name id and no name
      var userId = claimsPrincipal.Claims
        .Where(x => x.Type.Equals(ClaimTypes.NameIdentifier))
        .Select(x => x.Value)
        //should not be required
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Where(x => Guid.TryParse(input: x, out Guid g))
        .FirstOrDefault()
      ;
      //we are no longer going to rely on this
      //var name = claimsPrincipal.Claims
      //  .Where(x => x.Type.Equals(ClaimTypes.Name))
      //  .Select(x => x.Value)
      //  .FirstOrDefault()
      //;
      //was sometimes: {http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: foo@bar.baz}
      //now: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: userId}
      if (
        string.IsNullOrEmpty(userId)
      //&&
      //string.IsNullOrEmpty(iPrincipal.Identity.Name)
      //&&
      //string.IsNullOrEmpty(name)
      ) {
        return true;
      }
      return false;
    }



  }
}
