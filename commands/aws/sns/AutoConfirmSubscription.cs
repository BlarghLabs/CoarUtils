using System.Net;
using Amazon.SimpleNotificationService.Util;
using CoarUtils.commands.logging;
using RestSharp;

namespace CoarUtils.commands.aws.sns {
  public class AutoConfirmSubscription {
    public static async Task<bool> Execute(Message request, CancellationToken cancellationToken) {
      try {
        if (!string.IsNullOrEmpty(request.SubscribeURL)) {
          var uri = new Uri(request.SubscribeURL);
          var baseUrl = uri.GetLeftPart(System.UriPartial.Authority);
          var resource = request.SubscribeURL.Replace(baseUrl, "");
          var response = await new RestClient(baseUrl)
            .ExecuteAsync(new RestRequest {
              Resource = resource,
              Method = Method.Get,
              RequestFormat = DataFormat.Xml
            }, cancellationToken);
          if (response.StatusCode != HttpStatusCode.OK) {
            LogIt.W("unable to get message: " + response.StatusCode.ToString(), cancellationToken);
            return false;
          } else {
            LogIt.I(response.Content, cancellationToken);
            return true;
          }
        }
        return true;
      } catch (Exception ex) {
        LogIt.E(ex, cancellationToken);
        return false;
      }
    }
  }
}
