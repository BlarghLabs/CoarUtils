using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CoarUtils.commands.random.words {
  public static class GetRandomWordViaRandomWordApi {

    public class Request {
      public int total => 1; //{ get; set; }
    }

    public class Response : ResponseStatusModel {

      public List<string> words { get; set; } = new List<string> { };
    }

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken,
      WebProxy wp = null
    ) {
      var response = new Response { };

      try {
        #region validation 
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        #endregion
        var resource = $"/word?number={request.total}";

        var client = new RestClient("https://random-word-api.herokuapp.com/");
        var restRequest = new RestRequest(
          resource: resource,
          method: Method.Get
        );
        //if (wp != null) {
        //  client.Proxy = wp;
        //}
        var restResponse = await client.ExecuteAsync(restRequest);
        if (restResponse.ErrorException != null && !string.IsNullOrWhiteSpace(restResponse.ErrorException.Message)) {
          response.status = $"rest call had error exception: {restResponse.ErrorException.Message}";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          response.status = $"status code not OK {restResponse.StatusCode}";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }
        if (restResponse.ErrorException != null) {
          response.status = $"response had error exception: {restResponse.ErrorException.Message}";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          response.status = $"StatusCode was {restResponse.StatusCode}";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }
        if (string.IsNullOrWhiteSpace(restResponse.Content)) {
          response.status = $"content was empty";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }
        //["dorks","rosebud"]
        var content = restResponse.Content;
        dynamic json = JsonConvert.DeserializeObject(content);

        if ((json == null) || json.Count == 0) {
          response.status = $"there are no words";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }
        foreach (var w in json) {
          response.words.Add(w.Value);
        }

        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex);
        response.status = $"unexpected error";
        response.httpStatusCode = HttpStatusCode.InternalServerError;
        return response;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          response,
        }, Formatting.Indented));
      }
    }
  }
}

