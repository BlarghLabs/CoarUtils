﻿using System.Net;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CoarUtils.commands.gis.esri {

  //https://developers.arcgis.com/rest/geocode/api-reference/geocoding-reverse-geocode.htm
  //https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/reverseGeocode?f=pjson&featureTypes=&location=-117.205525,34.038232

  public static class ReverseGeocodeViaEsri {
    #region models
    public class Request {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
      public bool forStorage { get; set; }
      public string token { get; set; }
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
        #region validation
        if (request == null) {
          return response = new Response { status = "params were null" };
        }
        if ((request.lat == 0) && (request.lng == 0)) {
          return response = new Response { status = "lat and long ZERO" };
        }
        #endregion

        var resource = $"arcgis/rest/services/World/GeocodeServer/reverseGeocode?f=pjson&featureTypes=&location={request.lng.ToString()},{request.lat.ToString()}&forStorage={request.forStorage.ToString().ToLower()}&token={request.token}";
        var client = new RestClient("https://geocode.arcgis.com/");
        var restRequest = new RestRequest(resource, Method.Get);
        restRequest.RequestFormat = DataFormat.Json;
        var restResponse = await client.ExecuteAsync(restRequest);
        if (restResponse.ErrorException != null) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = restResponse.ErrorException.Message;
          return response;
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
        //var apiStatus = json.status.Value;
        //if (apiStatus != "OK") {
        //return response = new Response { status = $"api status result was {apiStatus}" };
        //}
        if (json.address == null) {
          return response = new Response { status = $"address not found" };
        }
        response.address = json.address.LongLabel.Value;
        if (string.IsNullOrWhiteSpace(response.address)) {
          return response = new Response { status = "unable to reverse geocode address (LongLabel was empty)" };
        }

        response.city = json.address.City.Value;
        response.state = json.address.RegionAbbr.Value;
        response.postalCode = json.address.Postal.Value;
        response.country = json.address.CountryCode.Value;

        var anonymizedAddressComponenets = new List<string> { response.city, response.state, response.postalCode, response.country }
          .Where(x => !string.IsNullOrWhiteSpace(x))
          .ToList()
        ;
        if (anonymizedAddressComponenets.Any()) {
          response.anonymizedAddress = String.Join(", ", anonymizedAddressComponenets);
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
        request.token = "DO_NOT_LOG";
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
 * {
 "address": {
  "Match_addr": "InlandPsych",
  "LongLabel": "InlandPsych, 255 Terracina Blvd, Redlands, CA, 92373, USA",
  "ShortLabel": "InlandPsych",
  "Addr_type": "POI",
  "Type": "Doctor",
  "PlaceName": "InlandPsych",
  "AddNum": "255",
  "Address": "255 Terracina Blvd",
  "Block": "",
  "Sector": "",
  "Neighborhood": "",
  "District": "",
  "City": "Redlands",
  "MetroArea": "",
  "Subregion": "San Bernardino County",
  "Region": "California",
  "RegionAbbr": "CA",
  "Territory": "",
  "Postal": "92373",
  "PostalExt": "",
  "CntryName": "United States",
  "CountryCode": "USA"
 },
 "location": {
  "x": -117.205525,
  "y": 34.038232,
  "spatialReference": {
   "wkid": 4326,
   "latestWkid": 4326
  }
 }
}
{
 "address": {
  "Match_addr": "Redlands Community Hospital",
  "LongLabel": "Redlands Community Hospital, 350 Terracina Blvd, Redlands, CA, 92373, USA",
  "ShortLabel": "Redlands Community Hospital",
  "Addr_type": "POI",
  "Type": "Hospital",
  "PlaceName": "Redlands Community Hospital",
  "AddNum": "350",
  "Address": "350 Terracina Blvd",
  "Block": "",
  "Sector": "",
  "Neighborhood": "Live Oak Canyon",
  "District": "",
  "City": "Redlands",
  "MetroArea": "Inland Empire",
  "Subregion": "San Bernardino County",
  "Region": "California",
  "RegionAbbr": "CA",
  "Territory": "",
  "Postal": "92373",
  "PostalExt": "",
  "CntryName": "United States",
  "CountryCode": "USA"
 },
 "location": {
  "x": -117.207440889547,
  "y": 34.037590344717,
  "spatialReference": {
   "wkid": 4326,
   "latestWkid": 4326
  }
 }
}
{
 "address": {
  "Match_addr": "1724-1762 W Fern Ave, Redlands, California, 92373",
  "LongLabel": "1724-1762 W Fern Ave, Redlands, CA, 92373, USA",
  "ShortLabel": "1724-1762 W Fern Ave",
  "Addr_type": "StreetAddress",
  "Type": "",
  "PlaceName": "",
  "AddNum": "1762",
  "Address": "1762 W Fern Ave",
  "Block": "",
  "Sector": "",
  "Neighborhood": "Live Oak Canyon",
  "District": "",
  "City": "Redlands",
  "MetroArea": "Inland Empire",
  "Subregion": "San Bernardino County",
  "Region": "California",
  "RegionAbbr": "CA",
  "Territory": "",
  "Postal": "92373",
  "PostalExt": "",
  "CntryName": "United States",
  "CountryCode": "USA"
 },
 "location": {
  "x": -117.206914219727,
  "y": 34.035699777877,
  "spatialReference": {
   "wkid": 4326,
   "latestWkid": 4326
  }
 }
}
 */ 