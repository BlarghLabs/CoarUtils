using Amazon.SimpleNotificationService.Util;
using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;
using RestSharp;
using System;
using System.Net;

namespace CoarUtils.commands.aws.sns {
  public class AutoConfirmSubscription {
    public static bool Execute(Message request) {
      try {
        if (!string.IsNullOrEmpty(request.SubscribeURL)) {
          var uri = new Uri(request.SubscribeURL);
          var baseUrl = uri.GetLeftPart(System.UriPartial.Authority);
          var resource = request.SubscribeURL.Replace(baseUrl, "");
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
