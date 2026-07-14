using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using CoarUtils.models.gis;
using RestSharp;

namespace CoarUtils.utils.gis.reversegeocode {

  //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY
  public static class ViaGoogle {
    #region models
    public class Request {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
      public string apiKey { get; set; }
    }
    public class Response : ResponseStatusModel {
      public AddressFields addressFields { get; set; }
    }
    #endregion

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { addressFields = new AddressFields { } };
      try {
        var resource = "maps/api/geocode/json?latlng=" + request.lat.ToString() + "," + request.lng.ToString() + "&key=" + request.apiKey;
        var client = new RestClient("https://maps.googleapis.com/");
        var restRequest = new RestRequest(resource, Method.Get);
        restRequest.RequestFormat = DataFormat.Json;
        var restResponse = await client.ExecuteAsync(restRequest, cancellationToken).ConfigureAwait(false);
        if (restResponse.ErrorException != null) {
          LogIt.W(restResponse.ErrorException);
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          LogIt.W(restResponse.StatusCode);
        }
        var content = restResponse.Content;
        dynamic json = Newtonsoft.Json.Linq.JObject.Parse(content);
        dynamic address_components = json.results[0].address_components;

        for (int i = 0; i < address_components.Count; i++) {
          var ac = address_components[i];
          var lot = ac.types;
          for (int j = 0; j < ac.types.Count; j++) {
            var t = (string)ac.types[j].Value;
            if (t.Equals("street_number")) {
              response.addressFields.line1 = ac.short_name.Value;
              break;
            }
          }
          for (int j = 0; j < ac.types.Count; j++) {
            var t = (string)ac.types[j].Value;
            if (t.Equals("route")) {
              response.addressFields.street = ac.short_name.Value;
              break;
            }
          }
          for (int j = 0; j < ac.types.Count; j++) {
            var t = (string)ac.types[j].Value;
            if (t.Equals("sublocality")) {
              response.addressFields.city = ac.short_name.Value;
              break;
            }
          }
          for (int j = 0; j < ac.types.Count; j++) {
            var t = (string)ac.types[j].Value;
            if (t.Equals("administrative_area_level_1")) {
              response.addressFields.statecode = ac.short_name.Value;
              response.addressFields.state = ac.long_name.Value;
              break;
            }
          }
          for (int j = 0; j < ac.types.Count; j++) {
            var t = (string)ac.types[j].Value;
            if (t.Equals("country")) {
              response.addressFields.countrycode = ac.short_name.Value;
              break;
            }
          }
          for (int j = 0; j < ac.types.Count; j++) {
            var t = (string)ac.types[j].Value;
            if (t.Equals("postal_code")) {
              response.addressFields.postal = ac.short_name.Value;
              break;
            }
          }

        }
        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        LogIt.W(ex.Message + "|" + request.lat + "|" + request.lng);
      }
      return response;
    }
  }
}
