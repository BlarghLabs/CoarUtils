using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace CoarUtils.commands.addresses.google
{

    //85s, issue w/ colon in resource
    public static class ValidateAddressViaWebRequest {
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

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Response response,
      Request request,
      CancellationToken cancellationToken
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
        var  encoding = new UTF8Encoding();
        var bytes = encoding.GetBytes(parsedContent);

        var newStream = http.GetRequestStream();
        newStream.Write(bytes, 0, bytes.Length);
        newStream.Close();

        var restResponse = http.GetResponse();

        var stream = restResponse.GetResponseStream();
        var sr = new StreamReader(stream);
        var content = sr.ReadToEnd();

        LogIt.I(content);

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
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