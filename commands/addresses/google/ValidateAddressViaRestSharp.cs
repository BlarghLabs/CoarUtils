using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Text;

namespace CoarUtils.commands.addresses.google {
  //again, taking 85s
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

        //85s
        var resource = $"./v1:validateAddress?alt=json&key={request.apiKey}";
        //var resource = $"/v1:validateAddress?alt=json&key={request.apiKey}";
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
            //languageCode = "en",
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
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

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

#region result
/*
 *{
  "result": {
    "verdict": {
      "inputGranularity": "SUB_PREMISE",
      "validationGranularity": "PREMISE",
      "geocodeGranularity": "PREMISE",
      "addressComplete": true,
      "hasUnconfirmedComponents": true,
      "hasInferredComponents": true
    },
    "address": {
      "formattedAddress": "2930 Pearl Street Suite 100, Boulder, CO 80301-1124, USA",
      "postalAddress": {
        "regionCode": "US",
        "languageCode": "en",
        "postalCode": "80301-1124",
        "administrativeArea": "CO",
        "locality": "Boulder",
        "addressLines": [
          "2930 Pearl St Suite 100"
        ]
      },
      "addressComponents": [
        {
          "componentName": {
            "text": "2930",
            "languageCode": "en"
          },
          "componentType": "street_number",
          "confirmationLevel": "CONFIRMED"
        },
        {
          "componentName": {
            "text": "Pearl Street",
            "languageCode": "en"
          },
          "componentType": "route",
          "confirmationLevel": "CONFIRMED"
        },
        {
          "componentName": {
            "text": "Suite 100",
            "languageCode": "en"
          },
          "componentType": "subpremise",
          "confirmationLevel": "UNCONFIRMED_BUT_PLAUSIBLE"
        },
        {
          "componentName": {
            "text": "Boulder",
            "languageCode": "en"
          },
          "componentType": "locality",
          "confirmationLevel": "CONFIRMED"
        },
        {
          "componentName": {
            "text": "CO",
            "languageCode": "en"
          },
          "componentType": "administrative_area_level_1",
          "confirmationLevel": "CONFIRMED"
        },
        {
          "componentName": {
            "text": "USA",
            "languageCode": "en"
          },
          "componentType": "country",
          "confirmationLevel": "CONFIRMED"
        },
        {
          "componentName": {
            "text": "80301"
          },
          "componentType": "postal_code",
          "confirmationLevel": "CONFIRMED",
          "inferred": true
        },
        {
          "componentName": {
            "text": "1124"
          },
          "componentType": "postal_code_suffix",
          "confirmationLevel": "CONFIRMED",
          "inferred": true
        }
      ],
      "unconfirmedComponentTypes": [
        "subpremise"
      ]
    },
    "geocode": {
      "location": {
        "latitude": 40.0216013,
        "longitude": -105.2545612
      },
      "plusCode": {
        "globalCode": "85GP2PCW+J5"
      },
      "bounds": {
        "low": {
          "latitude": 40.0214235,
          "longitude": -105.2552367
        },
        "high": {
          "latitude": 40.022141,
          "longitude": -105.2539148
        }
      },
      "featureSizeMeters": 83.12698,
      "placeId": "EjgyOTMwIFBlYXJsIFN0cmVldCBTdWl0ZSAxMDAsIEJvdWxkZXIsIENPIDgwMzAxLTExMjQsIFVTQSIfGh0KFgoUChIJAYG51dnta4cRKUh8ScmdvqgSAzEwMA",
      "placeTypes": [
        "subpremise"
      ]
    },
    "metadata": {
      "business": true,
      "poBox": false
    },
    "uspsData": {
      "standardizedAddress": {
        "firstAddressLine": "2930 PEARL ST STE 100",
        "cityStateZipAddressLine": "BOULDER CO 80301-1124",
        "city": "BOULDER",
        "state": "CO",
        "zipCode": "80301",
        "zipCodeExtension": "1124"
      },
      "deliveryPointCode": "30",
      "deliveryPointCheckDigit": "7",
      "dpvConfirmation": "S",
      "dpvFootnote": "AACC",
      "dpvCmra": "N",
      "dpvVacant": "N",
      "dpvNoStat": "N",
      "carrierRoute": "C009",
      "carrierRouteIndicator": "D",
      "postOfficeCity": "BOULDER",
      "postOfficeState": "CO",
      "fipsCountyCode": "013",
      "county": "BOULDER",
      "elotNumber": "0002",
      "elotFlag": "A",
      "addressRecordType": "S"
    }
  },
  "responseId": "3ac551e3-e317-4a78-b90b-1740339b4117"
}
 * */
#endregion