using System;
using System.Collections.Generic;
using System.Text;

namespace CoarUtils.commands.base64 {
  public static class Base64Encode {
    public static string Execute(string plainText) {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }
  }
}
