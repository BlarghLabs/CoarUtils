using System.Net;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CoarUtils.commands.gis.geoapify {
  public static class ReverseGeocodeViaGeoapify {
    #region models
    public class Request {
      public decimal latitude { get; set; }
      public decimal longitude { get; set; }
      public string apiKey { get; set; }
    }
    public class Response : ResponseStatusModel {
      public string formattedAddress { get; set; }
      public string anonymizedAddress { get; set; }
      public string addressLine1 { get; set; }
      public string addressLine2 { get; set; }
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
        #region validation
        if (request == null) {
          return response = new Response { status = "params were null" };
        }
        if ((request.latitude <= 0) && (request.longitude <= 0)) {
          return response = new Response { status = "lat and long ZERO" };
        }
        #endregion

        //https://api.geoapify.com/v1/geocode/reverse?lat=52.47944744483806&lon=13.213967739855434&format=json&apiKey=YOUR_API_KEY
        var resource = $"v1/geocode/reverse?lat={request.latitude.ToString()}&lon={request.longitude.ToString()}&format=json&apiKey={request.apiKey}";
        var client = new RestClient("https://api.geoapify.com/");
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
        if (json.results == null || json.results.Count == 0) {
          return response = new Response { status = $"no results returned" };
        }

        response.addressLine1 = json.results[0].address_line1?.Value;
        response.addressLine2 = json.results[0].address_line2?.Value;
        response.city = json.results[0].city?.Value;
        response.state = json.results[0].state_code?.Value; ;
        response.postalCode = json.results[0].postcode?.Value;
        response.country = json.results[0].country_code?.Value;
        response.formattedAddress = json.results[0].formatted?.Value;
        if(string.IsNullOrWhiteSpace(response.formattedAddress)) {
          return response = new Response { status = $"formatted (address) not found" };
        }
        var anonymizedAddressComponenets = new List<string> { 
            response.city, 
            response.state, 
            response.postalCode, 
            response.country 
          }.Where(x => !string.IsNullOrWhiteSpace(x))
          .ToList()
        ;
        if (anonymizedAddressComponenets.Any()) {
          response.anonymizedAddress = string.Join(", ", anonymizedAddressComponenets);
        }

        //could be more efficiently written - why is this necessary?
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
        return response = new Response { httpStatusCode = HttpStatusCode.InternalServerError, status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS };
      } finally {
        request.apiKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
            response,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented), cancellationToken);
      }
    }
  }
}


/*
 *
{
  "results": [
    {
      "datasource": {
        "sourcename": "openstreetmap",
        "attribution": "© OpenStreetMap contributors",
        "license": "Open Database License",
        "url": "https://www.openstreetmap.org/copyright"
      },
      "name": "Kinar Galil hotel",
      "country": "Israel",
      "country_code": "il",
      "state": "North District",
      "county": "Golan Subdistrict",
      "city": "Golan Regional Council",
      "postcode": "1294900",
      "street": "92",
      "iso3166_2": "IL-Z",
      "lon": 35.64480482488004,
      "lat": 32.86127705,
      "distance": 0,
      "result_type": "amenity",
      "state_code": "QU",
      "county_code": "GO",
      "formatted": "Kinar Galil hotel, 92, 1294900 Golan Regional Council, Israel",
      "address_line1": "Kinar Galil hotel",
      "address_line2": "92, 1294900 Golan Regional Council, Israel",
      "category": "accommodation.hotel",
      "timezone": {
        "name": "Asia/Jerusalem",
        "offset_STD": "+02:00",
        "offset_STD_seconds": 7200,
        "offset_DST": "+03:00",
        "offset_DST_seconds": 10800,
        "abbreviation_STD": "IST",
        "abbreviation_DST": "IDT"
      },
      "plus_code": "8G4QVJ6V+GW",
      "rank": {
        "importance": 0.00000999999999995449,
        "popularity": 2.2527728235256617
      },
      "place_id": "51d694e9f688d2414059ce458d533e6e4040f00102f90131da592600000000c002019203114b696e61722047616c696c20686f74656ce203216f70656e7374726565746d61703a76656e75653a7761792f363433343232373639",
      "bbox": {
        "lon1": 35.6432942,
        "lat1": 32.8595985,
        "lon2": 35.6463573,
        "lat2": 32.862852
      }
    }
  ],
  "query": {
    "lat": 32.86127705,
    "lon": 35.64480482488004,
    "plus_code": "8G4QVJ6V+GW"
  }
}
 */ 