//using System;
//using System.Security.Cryptography;
//using System.Text;

//namespace CoarUtils.commands.hashing.sha256 {
//  public static class GetSha256Hash {
//    public static string Execute(string value) {
//      var sb = new StringBuilder();
//      using (var hash = SHA256.Create()) {
//        var enc = Encoding.UTF8;
//        var ba = hash.ComputeHash(enc.GetBytes(value));
//        foreach (var b in ba)
//          sb.Append(b.ToString("x2"));
//      }
//      var base64 = CoarUtils.commands.base64.Base64Encode.Execute(sb.ToString());
//      return base64;
//    }
//  }
//}
