using System;
using System.Collections.Generic;
using System.Text;

namespace CoarUtils.Utils.GoogleAuth {
  public class GenerateGoogleOAuthUrl {
    public static string Execute(string clientId, string redirectUrl, List<string> listOfScopes) {
      return Execute(clientId, redirectUrl, string.Join("%20", listOfScopes.ToArray()));
    }

    public static string Execute(string clientId, string redirectUrl, string scopes) {
      //https://accounts.google.com/o/oauth2/auth?redirect_uri=https://developers.google.com/oauthplayground&response_type=code&client_id=XXX.apps.googleusercontent.com&scope=https://mail.google.com&approval_prompt=force&access_type=offline
      var url = "https://accounts.google.com/o/oauth2/auth?"
        + "redirect_uri=" + UrlEncodeForGoogle(redirectUrl)
        + "&response_type=code"
        + "&client_id=" + clientId
        + "&scope=" + scopes
        + "&approval_prompt=force"
        + "&access_type=offline";
      return url;

      //NOTE: Key piece here, from Andrew's reply -> access_type=offline forces a refresh token to be issued
      //string Url = "https://accounts.google.com/o/oauth2/auth?scope={0}&redirect_uri={1}&response_type={2}&client_id={3}&state={4}&access_type=offline&approval_prompt=force";
      //string scope = ""; //UrlEncodeForGoogle("https://www.googleapis.com/auth/calendar https://www.googleapis.com/auth/calendar.readonly").Replace("%20", "+");
      //foreach (string s in GetListOfScopes()) {
      //  scope += s + "+";
      //}
      //string redirect_uri_encode = UrlEncodeForGoogle(Globals.GoogleClientCallback);
      //string response_type = "code";
      //string state = "";

      //return string.Format(Url, scope, redirect_uri_encode, response_type, Globals.GoogleClientID, state);
    }

    public static string ExecuteNotOffline(string clientId, string redirectUrl, string scopes) {
      var url = "https://accounts.google.com/o/oauth2/auth?"
        + "redirect_uri=" + UrlEncodeForGoogle(redirectUrl)
        + "&response_type=code"
        + "&client_id=" + clientId
        + "&scope=" + scopes
      ;
      return url;
    }

    public static string UrlEncodeForGoogle(string url) {
      var unReservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
      var sb = new StringBuilder();

      foreach (char symbol in url) {
        if (unReservedChars.IndexOf(symbol) != -1) {
          sb.Append(symbol);
        } else {
          sb.Append('%' + String.Format("{0:X2}", (int)symbol));
        }
      }

      return sb.ToString();
    }
  }
}
