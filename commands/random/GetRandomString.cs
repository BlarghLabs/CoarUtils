using CoarUtils.commands.logging;
using Newtonsoft.Json;

namespace CoarUtils.commands.random {
  public static class GetRandomString {
    public class Request {
      public int length { get; set; } = 8;
    }

    public class Response : models.commands.ResponseStatusModel {
      public string result { get; set; }
    }

    public static string Execute(
      Request request
    ) {
      var result = "";
      try {
        var random = new Random();
        //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
        result = new string(Enumerable.Repeat(chars, request.length)
          .Select(s => s[random.Next(s.Length)])
          .ToArray())
        ;
        return result;
      } catch (Exception ex) {
        LogIt.I(ex, CancellationToken.None);
        throw;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          result,
          request,
        }, Formatting.Indented), CancellationToken.None);
      }
    }
  }
}

