using Amazon.SimpleNotificationService.Util;
using CoarUtils.commands.logging;
using RestSharp;
using System;
using System.Net;

namespace CoarUtils.commands.aws.sns {
  public class AutoConfirmSubscription {
    public static bool Execute(Message m) {
      try {
        if (!string.IsNullOrEmpty(m.SubscribeURL)) {
          var uri = new Uri(m.SubscribeURL);
          var baseUrl = uri.GetLeftPart(System.UriPartial.Authority);
          var resource = m.SubscribeURL.Replace(baseUrl, "");
          var response = new RestClient(baseUrl)
            .ExecuteAsync(new RestRequest {
              Resource = resource,
              Method = Method.Get,
              RequestFormat = DataFormat.Xml
            }).Result;
          if (response.StatusCode != HttpStatusCode.OK) {
            LogIt.W("unable to get message: " + response.StatusCode.ToString());
            return false;
          } else {
            LogIt.I(response.Content);
            return true;
          }
        }
        return true;
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }
    }
  }
}
