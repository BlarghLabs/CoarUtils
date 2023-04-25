﻿using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Text;

namespace CoarUtils.commands.addresses.google {

  public static class ValidateAddressViaRestSharp {
    public class Request {
      public string address { get; set; }
      public string apiKey { get; set; }
    }
    public class Response {
      public string formattedAddress { get; set; }
      public string addressLines { get; set; }
      public string locality { get; set; }
      public string administrativeArea { get; set; }
      public string postalCode { get; set; }
      public string regionCode { get; set; }
      public string sublocality { get; set; }
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
        if (string.IsNullOrWhiteSpace(request.address)) {
          hsc = HttpStatusCode.BadRequest;
          status = "address required";
          return;
        }
        if (string.IsNullOrWhiteSpace(request.apiKey)) {
          hsc = HttpStatusCode.BadRequest;
          status = "apiKey required";
          return;
        }

        var resource = $"./v1:validateAddress?alt=json&key={request.apiKey}";
        //var resource = $"v1:validateAddress?alt=json&key={request.apiKey}";
        //var resource = $"/v1%3AvalidateAddress?alt=json&key={request.apiKey}";
        
        var client = new RestClient("https://addressvalidation.googleapis.com/");
        //var client = new RestClient("https://content-addressvalidation.googleapis.com/");
        
        var restRequest = new RestRequest {
          Resource = resource,
          Method = Method.Post,
          RequestFormat = DataFormat.Json,
        };
        restRequest.AddJsonBody(new {
          address = new {
            addressLines = new string[] {
              request.address
            },
            //languageCode = "en"
          },
          previousResponseId = "",
          enableUspsCass = false
        });
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

        //response.address = json.results[0].formatted_address.Value;

        //foreach (var results in json.results) {
        //  if (results.address_components != null) {
        //    foreach (var address_component in results.address_components) {
        //      var types = ((JArray)address_component.types).ToList();
        //      if (types.Any(x => (string)x == "postal_code")) {
        //        response.postalCode = address_component.long_name.Value;
        //      }
        //      if (types.Any(x => (string)x == "locality")) {
        //        response.city = address_component.long_name.Value;
        //      }
        //      if (types.Any(x => (string)x == "administrative_area_level_1")) {
        //        response.state = address_component.long_name.Value;
        //      }
        //      if (types.Any(x => (string)x == "country")) {
        //        response.country = address_component.long_name.Value;
        //      }
        //    }
        //    var anonymizedAddressComponenets = new List<string> { response.city, response.state, response.postalCode, response.country }
        //      .Where(x => !string.IsNullOrWhiteSpace(x))
        //      .ToList()
        //    ;
        //    if (anonymizedAddressComponenets.Any()) {
        //      response.anonymizedAddress = String.Join(", ", anonymizedAddressComponenets);
        //      break;
        //    }
        //  }
        //  if (!string.IsNullOrWhiteSpace(response.anonymizedAddress)) {
        //    break;
        //  }
        //}

        ////could be more efficiently written
        //if (!string.IsNullOrWhiteSpace(response.anonymizedAddress)) {
        //  response.anonymizedAddress = response.anonymizedAddress
        //    .Replace(", , ,", ",")
        //    .Replace(", ,", ",")
        //    .Trim()
        //  ;
        //  if (response.anonymizedAddress.StartsWith(",")) {
        //    response.anonymizedAddress = response.anonymizedAddress.Substring(1);
        //  }
        //}

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