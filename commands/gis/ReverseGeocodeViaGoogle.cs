using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace CoarUtils.commands.gis {

  //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY
  public static class ReverseGeocodeViaGoogle {
    public class Request {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
      public string apiKey { get; set; }
    }
    public class Response {
      public string address { get; set; }
      public string anonymizedAddress { get; set; }
      public string city { get; set; }
      public string state { get; set; }
      public string postalCode { get; set; }
      public string country { get; set; }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Response response,
      Request request,
      CancellationToken? ct = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      response = new Response { };
      try {
        if (request == null) {
          hsc = HttpStatusCode.BadRequest;
          status = "params were null";
          return;
        }
        if ((request.lat == 0) && (request.lng == 0)) {
          hsc = HttpStatusCode.BadRequest;
          status = "lat and long ZERO";
          return;
        }
        var resource = "maps/api/geocode/json?latlng=" + request.lat.ToString() + "," + request.lng.ToString() + "&key=" + request.apiKey;
        var client = new RestClient("https://maps.googleapis.com/");
        var restRequest = new RestRequest(resource, Method.Get);
        restRequest.RequestFormat = DataFormat.Json;
        var restResponse = client.ExecuteAsync(restRequest).Result;
        if (restResponse.ErrorException != null) {
          hsc = HttpStatusCode.BadRequest;
          status = restResponse.ErrorException.Message;
          return;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          hsc = HttpStatusCode.BadRequest;
          status = $"status was {restResponse.StatusCode.ToString()}";
          return;
        }
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
        var content = restResponse.Content;
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
        response.address = json.results[0].formatted_address.Value;

        foreach (var results in json.results) {
          if (results.address_components != null) {
            foreach (var address_component in results.address_components) {
              var types = ((JArray)address_component.types).ToList();
              if (types.Any(x => (string)x == "postal_code")) {
                response.postalCode = address_component.long_name.Value;
              }
              if (types.Any(x => (string)x == "locality")) {
                response.city = address_component.long_name.Value;
              }
              if (types.Any(x => (string)x == "administrative_area_level_1")) {
                response.state = address_component.long_name.Value;
              }
              if (types.Any(x => (string)x == "country")) {
                response.country = address_component.long_name.Value;
              }
            }
            var anonymizedAddressComponenets = new List<string> { response.city, response.state, response.postalCode, response.country }
              .Where(x => !string.IsNullOrWhiteSpace(x))
              .ToList()
            ;
            if (anonymizedAddressComponenets.Any()) {
              response.anonymizedAddress = String.Join(", ", anonymizedAddressComponenets);
              break;
            }
          }
          if (!string.IsNullOrWhiteSpace(response.anonymizedAddress)) {
            break;
          }
        }

        //could be more efficiently written
        if (!string.IsNullOrWhiteSpace(response.anonymizedAddress)) {
          response.anonymizedAddress = response.anonymizedAddress
            .Replace(", , ,", ",")
            .Replace(", ,", ",")
            .Trim()
          ;
          if (response.anonymizedAddress.StartsWith(",")) {
            response.anonymizedAddress = response.anonymizedAddress.Substring(1);
          }
        }

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexecpected error";
        return;
      } finally {
        request.apiKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}