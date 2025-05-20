using System.Net;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CoarUtils.commands.gis.google {

  //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY
  public static class ReverseGeocodeViaGoogle {
    #region models
    public class Request {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
      public string apiKey { get; set; }
    }
    public class Response : models.commands.ResponseStatusModel {
      public string address { get; set; }
      public string anonymizedAddress { get; set; }
      public string city { get; set; }
      public string state { get; set; }
      public string postalCode { get; set; }
      public string country { get; set; }
    }
    #endregion

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };
      try {
        if (request == null) {
          return response = new Response { status = "params were null" };
        }
        if (request.lat == 0 && request.lng == 0) {

          return response = new Response { status = "lat and long ZERO" };

        }
        var resource = "maps/api/geocode/json?latlng=" + request.lat.ToString() + "," + request.lng.ToString() + "&key=" + request.apiKey;
        var client = new RestClient("https://maps.googleapis.com/");
        var restRequest = new RestRequest(resource, Method.Get);
        restRequest.RequestFormat = DataFormat.Json;
        var restResponse = await client.ExecuteAsync(restRequest);
        if (restResponse.ErrorException != null) {
          return response = new Response { status = restResponse.ErrorException.Message };
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          return response = new Response { status = $"status was {restResponse.StatusCode.ToString()}" };
        }
        if (restResponse.ErrorException != null && !string.IsNullOrWhiteSpace(restResponse.ErrorException.Message)) {
          return response = new Response { status = $"rest call had error exception: {restResponse.ErrorException.Message}" };
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          return response = new Response { status = $"status code not OK {restResponse.StatusCode}" };
        }
        var content = restResponse.Content;
        dynamic json = JObject.Parse(content);
        var apiStatus = json.status.Value;
        if (apiStatus != "OK") {
          return response = new Response { status = $"api status result was {apiStatus}" };
        }
        if (json.results.Count == 0) {
          return response = new Response { status = $"results count was ZERO" };
        }
        response.address = json.results[0].formatted_address.Value;
        if (string.IsNullOrWhiteSpace(response.address)) {
          return response = new Response { status = "unable to reverse geocode address (address was empty)" };
        }

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
              response.anonymizedAddress = string.Join(", ", anonymizedAddressComponenets);
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

        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new Response { status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS, httpStatusCode = HttpStatusCode.InternalServerError };
      } finally {
        request.apiKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented), cancellationToken);
      }
    }
  }
}