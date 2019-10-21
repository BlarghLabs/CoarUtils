using System;
using System.Collections.Generic;
using System.Text;

namespace CoarUtils.commands.base64 {
  public static class Base64Decode {
    public static string Execute(string base64EncodedData) {
      var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
      return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
  }
}
