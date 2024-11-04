using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;
using RestSharp;
using System.Net;

namespace CoarUtils.commands.slack {

  public class PublishToChannel {
    public const string BASE = "https://hooks.slack.com";
    public const string DEFAULT_HEADER = "BORP";

    public static void ExecuteAsync(
      string content,
      string resource,
      string header = DEFAULT_HEADER
    ) {
      try {
        Task.Factory.StartNew(() => {
          try {
            if (!ExecuteSync(content: content, resource: resource, header: header)) {
              LogIt.W("unable to publish to slack");
            }
          } catch (Exception ex) {
            LogIt.E(ex);
          }
        });
      } catch (Exception ex) {
        LogIt.E(ex);
      }
    }


    public static bool ExecuteSync(
      string content,
      string resource,
      string header = DEFAULT_HEADER
    ) {
      //TODO: do this async in task
      try {
        var h = "----------" + (string.IsNullOrEmpty(header) ? DEFAULT_HEADER : header.ToUpper()) + "----------" + "\n";
        var client = new RestClient(BASE);
        resource = resource.Replace(BASE, "");
        var restRequest = new RestRequest {
          Resource = resource,
          Method = Method.Post,
          RequestFormat = DataFormat.Json,
        };
        content = h + content.Replace("\"", "'");
        restRequest.AddJsonBody(new {
          text = content
        });

        var restResponse = client.ExecuteAsync(restRequest).Result;

        //var content = response.Content;
        if (restResponse.ErrorException != null) {
          throw restResponse.ErrorException;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          LogIt.W(restResponse.StatusCode);
        }
        return (restResponse.StatusCode == HttpStatusCode.OK);
      } catch (Exception ex) {
        LogIt.E(ex);
      }
      return false;
    }
  }
}
