using CoarUtils.commands.logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace CoarUtils.commands.random.words {
  public static class GetRandomWordViaRandomWordApi {

    public class request {
      public int total => 1; //{ get; set; }
    }

    public class response {
      public response() {
        words = new List<string> { };
      }
      public List<string> words { get; set; }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out response r,
      request m,
      CancellationToken? ct = null,
      WebProxy wp = null
    ) {
      r = new response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = "cancellation requested";
          return;
        }

        var resource = $"/word?number={@m.total}";

        var client = new RestClient("https://random-word-api.herokuapp.com/");
        var request = new RestRequest(
          resource: resource,
          method: Method.Get
        );
        //if (wp != null) {
        //  client.Proxy = wp;
        //}
        var response = client.ExecuteAsync(request).Result;

        if (response.ErrorException != null) {
          status = $"response had error exception: {response.ErrorException.Message}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (response.StatusCode != HttpStatusCode.OK) {
          status = $"StatusCode was {response.StatusCode}";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (string.IsNullOrWhiteSpace(response.Content)) {
          status = $"content was empty";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        //["dorks","rosebud"]
        var content = response.Content;
        dynamic json = JsonConvert.DeserializeObject(content);

        if ((json == null) || json.Count == 0) {
          status = $"there are no words";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        foreach (var w in json) {
          r.words.Add(w.Value);
        }

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = "task cancelled";
          return;
        }

        status = $"unexpected error";
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex);
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          r,

        }, Formatting.Indented));
      }
    }
  }
}

