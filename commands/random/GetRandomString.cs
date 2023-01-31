using System;
using System.Linq;
using CoarUtils.commands.logging;
using Newtonsoft.Json;

namespace CoarUtils.commands.random {
  public static class GetRandomString {
    public class Request {
      public Request() {
        length = 8;
      }
      public int length { get; set; }
    }

    public class Response {
      public Response() {
      }
      public string result { get; set; }
    }

    public static string Execute(
      Request m
    ) {
      var result = "";
      try {
        var random = new Random();
        //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
        result = new string(Enumerable.Repeat(chars, m.length)
          .Select(s => s[random.Next(s.Length)])
          .ToArray())
        ;
        return result;
      } catch (Exception ex) {
        LogIt.E(ex);
        throw;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          result,
          m,
        }, Formatting.Indented));
      }
    }
  }
}

