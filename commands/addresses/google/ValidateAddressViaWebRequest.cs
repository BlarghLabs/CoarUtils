using System.Net;
using System.Text;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;

namespace CoarUtils.commands.addresses.google {

  /// <summary>
  /// TARGET command shape - see CLAUDE.md "Legacy -> Target Migration". Converted 2026-09-02.
  ///
  /// The whole HTTP exchange was synchronous - GetRequestStream, GetResponse and ReadToEnd all
  /// block a thread on a round trip to Google. Every one of them is awaited now. The header comment
  /// says "85s", so this is precisely the call you do not want holding a thread-pool thread.
  ///
  /// The response object is still not populated from the returned JSON - it never was; the content
  /// only reaches the log. Left as found rather than quietly changing what this returns, but it is
  /// why every field on Response is always null.
  /// </summary>
  public static class ValidateAddressViaWebRequest {
    public class Request {
      public string address { get; set; }
      public string apiKey { get; set; }
    }
    public class Response : ResponseStatusModel {
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
          return response = new Response { status = "params not found" };
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
        var http = (HttpWebRequest)WebRequest.Create(new Uri($"{baseUrl}{resource}"));
        http.Accept = "application/json";
        http.ContentType = "application/json";
        http.Method = "POST";

        string parsedContent = JsonConvert.SerializeObject(payload);
        var encoding = new UTF8Encoding();
        var bytes = encoding.GetBytes(parsedContent);

        using (var requestStream = await http.GetRequestStreamAsync()) {
          await requestStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        using (var webResponse = await http.GetResponseAsync())
        using (var stream = webResponse.GetResponseStream())
        using (var streamReader = new StreamReader(stream)) {
          var content = await streamReader.ReadToEndAsync(cancellationToken);
          LogIt.I(content, cancellationToken);
        }

        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (OperationCanceledException) {
        return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new Response {
          httpStatusCode = HttpStatusCode.InternalServerError,
          status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS,
        };
      } finally {
        // The api key is a credential and must never reach a log.
        if (request != null) {
          request.apiKey = "DO_NOT_LOG";
        }

        LogIt.I(JsonConvert.SerializeObject(
          new {
            response.httpStatusCode,
            response.status,
            request,
          }, Formatting.None), cancellationToken);
      }
    }
  }
}
