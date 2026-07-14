using System.Net;
using CoarUtils.commands.logging;
using RestSharp;

namespace CoarUtils.commands.slack {

  public class PublishToChannel {
    public const string BASE = "https://hooks.slack.com";
    public const string DEFAULT_HEADER = "BORP";

    // Compat overload for callers ported from MoarUtils that didn't pass a CancellationToken.
    public static void ExecuteAsync(
      string content,
      string resource,
      string header = DEFAULT_HEADER
    ) {
      ExecuteAsync(content, resource, CancellationToken.None, header);
    }

    // Compat overload for Execute without cancellationToken.
    public static async Task<bool> Execute(
      string content,
      string resource,
      string header = DEFAULT_HEADER
    ) {
      return await Execute(content, resource, CancellationToken.None, header).ConfigureAwait(false);
    }

    // Fire-and-forget: the caller does not care about the outcome and must not wait on the POST.
    public static void ExecuteAsync(
      string content,
      string resource,
      CancellationToken cancellationToken,
      string header = DEFAULT_HEADER
    ) {
      try {
        _ = Task.Run(async () => {
          try {
            if (!await Execute(content: content, resource: resource, header: header, cancellationToken: cancellationToken).ConfigureAwait(false)) {
              LogIt.W("unable to publish to slack", cancellationToken);
            }
          } catch (Exception ex) {
            LogIt.E(ex, cancellationToken);
          }
        });
      } catch (Exception ex) {
        LogIt.E(ex, cancellationToken);
      }
    }


    // Awaitable: the caller needs to know the post landed (and, at shutdown, that it landed before exiting).
    public static async Task<bool> Execute(
      string content,
      string resource,
      CancellationToken cancellationToken,
      string header = DEFAULT_HEADER
    ) {
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

        var restResponse = await client.ExecuteAsync(restRequest, cancellationToken).ConfigureAwait(false);

        //var content = response.Content;
        if (restResponse.ErrorException != null) {
          throw restResponse.ErrorException;
        }
        if (restResponse.StatusCode != HttpStatusCode.OK) {
          LogIt.W(restResponse.StatusCode, cancellationToken);
        }
        return (restResponse.StatusCode == HttpStatusCode.OK);
      } catch (Exception ex) {
        LogIt.E(ex, cancellationToken);
      }
      return false;
    }
  }
}
