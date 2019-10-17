using CoarUtils.commands.logging;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

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
        var request = new RestRequest {
          Resource = resource,
          Method = Method.POST,
          RequestFormat = DataFormat.Json,
        };
        content = h + content.Replace("\"", "'");
        request.AddJsonBody(new {
          text = content
        });

        var response = client.Execute(request);
        //var content = response.Content;
        if (response.ErrorException != null) {
          throw response.ErrorException;
        }
        if (response.StatusCode != HttpStatusCode.OK) {
          LogIt.W(response.StatusCode);
        }
        return (response.StatusCode == HttpStatusCode.OK);
      } catch (Exception ex) {
        LogIt.E(ex);
      }
      return false;
    }
  }
}
