using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace CoarUtils.commands.web {
  /// <summary>
  /// User impersonation support. In ASP.NET Core, FormsAuthentication is not available.
  /// This implementation stores impersonation state in session instead.
  /// </summary>
  public class UserImpersonation {
    private const string SESSION_KEY_PREV_USER = "Impersonation_PrevUser";
    private const string SESSION_KEY_RETURN_URL = "Impersonation_ReturnUrl";

    public static void ImpersonateUser(HttpContext context, string userName) {
      ImpersonateUser(context, userName, string.Empty);
    }

    public static void ImpersonateUser(HttpContext context, string userName, string returnUrl) {
      if (context == null)
        throw new InvalidOperationException("No HttpContext available. Unable to impersonate user.");
      if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
        throw new InvalidOperationException("No user is currently authenticated. Unable to impersonate user.");

      // Store the current user's name and return URL in session
      context.Session.SetString(SESSION_KEY_PREV_USER, context.User.Identity.Name ?? "");
      context.Session.SetString(SESSION_KEY_RETURN_URL, returnUrl ?? "");

      // The actual sign-in as the impersonated user must be done by the caller
      // using the ASP.NET Core authentication system (e.g., SignInManager)
    }

    public static string Deimpersonate(HttpContext context, bool redirect = true) {
      if (context == null)
        throw new InvalidOperationException("No HttpContext available. Unable to complete operation.");

      var prevUser = context.Session.GetString(SESSION_KEY_PREV_USER);
      if (string.IsNullOrEmpty(prevUser))
        return null;

      var returnUrl = context.Session.GetString(SESSION_KEY_RETURN_URL);

      // Clear impersonation session data
      context.Session.Remove(SESSION_KEY_PREV_USER);
      context.Session.Remove(SESSION_KEY_RETURN_URL);

      // The actual sign-in as the previous user must be done by the caller
      // using the ASP.NET Core authentication system (e.g., SignInManager)

      if (!string.IsNullOrWhiteSpace(returnUrl) && redirect) {
        return returnUrl;
      }
      return null;
    }

    public static string GetPrevUserName(HttpContext context) {
      if (context == null)
        throw new InvalidOperationException("No HttpContext available. Unable to complete operation.");

      return context.Session.GetString(SESSION_KEY_PREV_USER) ?? string.Empty;
    }

    public static bool GetIsImpersonating(HttpContext context) {
      if (context == null)
        throw new InvalidOperationException("No HttpContext available. Unable to complete operation.");

      return !string.IsNullOrEmpty(context.Session.GetString(SESSION_KEY_PREV_USER));
    }
  }
}
