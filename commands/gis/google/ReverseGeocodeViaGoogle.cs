using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace CoarUtils.commands.gis.google
{

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
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = "params were null";
          return response;
        }
        if (request.lat == 0 && request.lng == 0) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = "lat and long ZERO";
          return response;
        }
        var resource = "maps/api/geocode/json?latlng=" + request.lat.ToString() + "," + request.lng.ToString() + "&key=" + request.apiKey;
        var client = new RestClient("https://maps.googleapis.com/");
        var restRequest = new RestRequest(resource, Method.Get);
        restRequest.RequestFormat = DataFormat.Json;
        var restResponse = await client.ExecuteAsync(restRequest);
        if (restResponse.ErrorException != null) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = restResponse.ErrorException.Message;
          return response;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = $"status was {restResponse.StatusCode.ToString()}";
          return response;
        }
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
        var content = restResponse.Content;
        dynamic json = JObject.Parse(content);
        var apiStatus = json.status.Value;
        if (apiStatus != "OK") {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = $"api status result was {apiStatus}";
          return response;
        }
        if (json.results.Count == 0) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = $"results count was ZERO";
          return response;
        }
        response.address = json.results[0].formatted_address.Value;
        if (string.IsNullOrWhiteSpace(response.address)) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = "unable to reverse geocode address (address was empty)";
          return response;
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

        LogIt.E(ex);
        response.httpStatusCode = HttpStatusCode.InternalServerError;
        response.status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return response;
      } finally {
        request.apiKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}