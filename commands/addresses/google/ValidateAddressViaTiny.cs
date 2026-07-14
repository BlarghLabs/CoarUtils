using System.Net;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Tiny.RestClient;

namespace CoarUtils.commands.addresses.google {

  //85s, issue w/ colon in resource
  public static class ValidateAddressViaTiny {
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

        var payload = new {
          address = new {
            addressLines = new string[] {
                request.address
              },
          },
          previousResponseId = "",
          enableUspsCass = false
        };
        var baseUrl = "https://addressvalidation.googleapis.com/";
        var resource = $"v1:validateAddress?alt=json&key={request.apiKey}";
        var client = new TinyRestClient(new HttpClient(), $"{baseUrl}");
        //client.Settings.DefaultHeaders.Add("Content-Type", "application/json");
        //85s
        var restResponse = await client.PostRequest($"{resource}", payload).ExecuteAsStringAsync(cancellationToken).ConfigureAwait(false);

        LogIt.I(restResponse, cancellationToken);

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