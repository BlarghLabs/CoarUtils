using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading;

namespace CoarUtils.commands.gis {

  //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY
  public static class ReverseGeocodeViaGoogle {
    public class request {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
      public string apiKey { get; set; }
    }
    public class response {
      public string address { get; set; }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out response r,
      request m,
      CancellationToken? ct = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      r = new response { };
      try {
        if (m == null) {
          hsc = HttpStatusCode.BadRequest;
          status = "params were null";
          return;
        }
        if ((m.lat == 0) && (m.lng == 0)) {
          hsc = HttpStatusCode.BadRequest;
          status = "lat and long ZERO";
          return;
        }
        var resource = "maps/api/geocode/json?latlng=" + m.lat.ToString() + "," + m.lng.ToString() + "&key=" + m.apiKey;
        var client = new RestClient("https://maps.googleapis.com/");
        var request = new RestRequest(resource, Method.GET);
        request.RequestFormat = DataFormat.Json;
        var response = client.Execute(request);
        if (response.ErrorException != null) {
          hsc = HttpStatusCode.BadRequest;
          status = response.ErrorException.Message;
          return;
        }
        if (response.StatusCode != HttpStatusCode.OK) {
          hsc = HttpStatusCode.BadRequest;
          status = $"status was {response.StatusCode.ToString()}";
          return;
        }
        var content = response.Content;
        dynamic json = JObject.Parse(content);
        var apiStatus = json.status.Value;
        if (apiStatus != "OK") {
          hsc = HttpStatusCode.BadRequest;
          status = $"api status result was {apiStatus}";
          return;
        }
        if (json.results.Count == 0) {
          hsc = HttpStatusCode.BadRequest;
          status = $"results count was ZERO";
          return;
        }
        r.address = json.results[0].formatted_address.Value;

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexecpected error";
        return;
      } finally {
        m.apiKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            m,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}