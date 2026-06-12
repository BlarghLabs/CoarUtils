using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CoarUtils.Utils.GoogleAuth {
  public class ExchangeCodeForTokens {
    public class Request {
      public string code { get; set; }
      public string clientId { get; set; }
      public string clientSecret { get; set; }
      public string redirectUrl { get; set; }
      public bool includeEmptyScope { get; set; } = true;
    }

    public class Response : ResponseStatusModel {
      //{
      //  "access_token" : "XXX",
      //  "token_type" : "Bearer",
      //  "expires_in" : 3600,
      //  "refresh_token" : "1/XXX"
      //}

      public string access_token { get; set; }
      public string token_type { get; set; }
      public string refresh_token { get; set; }
      public int expires_in { get; set; }
    }

    /// <summary>
    /// On success save refresh and authtoken
    /// </summary>
    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };
      try {
        //POST /o/oauth2/token HTTP/1.1
        //Host: accounts.google.com
        //Content-length: 250
        //content-type: application/x-www-form-urlencoded
        //user-agent: google-oauth-playground

        //code=XXX&redirect_uri=https%3A%2F%2Fdevelopers.google.com%2Foauthplayground&client_id=XXX.apps.googleusercontent.com&scope=&client_secret=************&grant_type=authorization_code

        var restRequest = new RestRequest("o/oauth2/token", Method.Post);
        restRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddParameter("code", request.code);
        restRequest.AddParameter("redirect_uri", request.redirectUrl);
        restRequest.AddParameter("client_id", request.clientId);
        if (request.includeEmptyScope) {
          restRequest.AddParameter("scope", "");
        }
        restRequest.AddParameter("client_secret", request.clientSecret);
        restRequest.AddParameter("grant_type", "authorization_code");

        var client = (new RestClient("https://accounts.google.com/"));
        var restResponse = await client.ExecuteAsync(restRequest, cancellationToken).ConfigureAwait(false);
        var content = restResponse.Content;

        //valid response: 
        //{
        //  "access_token" : "XXX",
        //  "token_type" : "Bearer",
        //  "expires_in" : 3600,
        //  "refresh_token" : "XXX"
        //}

        if (restResponse.StatusCode != HttpStatusCode.OK) {
          return response = new Response { status = restResponse.StatusCode.ToString() + "|" + restResponse.Content };
        }

        //LogIt.D(content);
        dynamic json = JObject.Parse(content);
        response = new Response {
          access_token = json.access_token,
          expires_in = json.expires_in,
          refresh_token = json.refresh_token,
          token_type = json.token_type
        };

        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex);
        return response = new Response { status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS, httpStatusCode = HttpStatusCode.InternalServerError };
      } finally {
        // SECURITY: never log the response (access_token/refresh_token are credentials) or the raw
        // auth code / clientSecret. Log only status + non-secret request context, and surface whether
        // tokens came back as booleans (the signal we actually need to debug downstream GetUserInfo).
        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          request = new {
            request.clientId,
            request.redirectUrl,
            request.includeEmptyScope,
            codePresent = !string.IsNullOrWhiteSpace(request.code),
          },
          refreshTokenReturned = !string.IsNullOrWhiteSpace(response.refresh_token),
          accessTokenReturned = !string.IsNullOrWhiteSpace(response.access_token),
        }, Formatting.Indented));
      }
    }
  }
}
