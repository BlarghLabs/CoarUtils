using CoarUtils.commands.logging;
using CoarUtils.models;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CoarUtils.commands.random.words {
  public static class GetRandomWordViaRandomWordApi {

    public class Request {
      public int total => 1; //{ get; set; }
    }

    public class Response : CoarUtils.models.ResponseStatusModel {
      public List<string> words { get; set; } = new List<string> { };
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Response response,
      Request request,
      CancellationToken? ct = null,
      WebProxy wp = null
    ) {
      response = new Response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        var resource = $"/word?number={request.total}";

        var client = new RestClient("https://random-word-api.herokuapp.com/");
        var restRequest = new RestRequest(
          resource: resource,
          method: Method.Get
        );
        //if (wp != null) {
        //  client.Proxy = wp;
        //}
        var restResponse = client.ExecuteAsync(restRequest).Result;
        if (restResponse.ErrorException != null && !string.IsNullOrWhiteSpace(restResponse.ErrorException.Message)) {
          status = $"rest call had error exception: {restResponse.ErrorException.Message}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          status = $"status code not OK {restResponse.StatusCode}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (restResponse.ErrorException != null) {
          status = $"response had error exception: {restResponse.ErrorException.Message}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          status = $"StatusCode was {restResponse.StatusCode}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (string.IsNullOrWhiteSpace(restResponse.Content)) {
          status = $"content was empty";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        //["dorks","rosebud"]
        var content = restResponse.Content;
        dynamic json = JsonConvert.DeserializeObject(content);

        if ((json == null) || json.Count == 0) {
          status = $"there are no words";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        foreach (var w in json) {
          response.words.Add(w.Value);
        }

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        status = $"unexpected error";
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex);
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          response,
        }, Formatting.Indented));
      }
    }
  }
}

