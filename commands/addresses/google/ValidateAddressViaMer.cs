using System.Net;
using System.Text;
using CoarUtils.commands.logging;
using Newtonsoft.Json;

namespace CoarUtils.commands.addresses.google {

  public static class ValidateAddressViaMer {
    public class Request {
      public string address { get; set; }
      public string apiKey { get; set; }
    }
    public class Response : models.commands.ResponseStatusModel {
      public string formattedAddress { get; set; }
      public string addressLines { get; set; }
      public string locality { get; set; }
      public string administrativeArea { get; set; }
      public string postalCode { get; set; }
      public string regionCode { get; set; }
      public string sublocality { get; set; }
    }

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };
      try {
        if (request == null) {
          return response = new Response { status = "params were null" };
        }
        if (string.IsNullOrWhiteSpace(request.address)) {
          return response = new Response { status = "address required" };
        }
        if (string.IsNullOrWhiteSpace(request.apiKey)) {
          return response = new Response { status = "apiKey required" };
        }

        var httpClient = new HttpClient();
        var data = new {
          address = new {
            addressLines = new string[] {
                request.address
              },
          },
          previousResponseId = "",
          enableUspsCass = false
        };
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var baseUrl = "https://addressvalidation.googleapis.com/";
        var resource = $"v1:validateAddress?alt=json&key={request.apiKey}";
        //var resource = $"v1%3AvalidateAddress?alt=json&key={request.apiKey}";
        //var resource = HttpUtility.UrlEncode($"v1:validateAddress?alt=json&key={request.apiKey}");
        var restResponse = await httpClient.PostAsync($"{baseUrl}{resource}", content, cancellationToken).ConfigureAwait(false);

        var responseString = await restResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        LogIt.I(responseString, cancellationToken);

        ////https://content-addressvalidation.googleapis.com/v1:validateAddress?alt=json&key=KEY_HERE
        ////var resource = $"./v1:validateAddress?alt=json&key={request.apiKey}";
        //var resource = $"/v1%3AvalidateAddress?alt=json&key={request.apiKey}";
        //var client = new RestClient("https://addressvalidation.googleapis.com/");
        ////var client = new RestClient("https://content-addressvalidation.googleapis.com/");
        //var restRequest = new RestRequest {
        //  Resource =  resource,
        //  Method = Method.Post,
        //  RequestFormat = DataFormat.Json,
        //};
        //restRequest.AddJsonBody(new {
        //  address = new {
        //    addressLines = new string[] { 
        //      request.address 
        //    },
        //    //languageCode = "en"
        //  },
        //  previousResponseId = "",
        //  enableUspsCass = false
        //});
        //var restResponse = client.ExecuteAsync(restRequest).Result;
        //if (restResponse.ErrorException != null) {
        //  hsc = HttpStatusCode.BadRequest;
        //  status = restResponse.ErrorException.Message;
        //  return;
        //}
        //if (restResponse.StatusCode != HttpStatusCode.OK) {
        //  hsc = HttpStatusCode.BadRequest;
        //  status = $"status was {restResponse.StatusCode.ToString()}";
        //  return;
        //}
        //var content = restResponse.Content;
        //dynamic json = JObject.Parse(content);
        //var apiStatus = json.status.Value;
        //if (apiStatus != "OK") {
        //  hsc = HttpStatusCode.BadRequest;
        //  status = $"api status result was {apiStatus}";
        //  return;
        //}
        //if (json.results.Count == 0) {
        //  hsc = HttpStatusCode.BadRequest;
        //  status = $"results count was ZERO";
        //  return;
        //}

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