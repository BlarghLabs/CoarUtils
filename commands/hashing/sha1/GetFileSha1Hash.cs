using System.Security.Cryptography;

namespace CoarUtils.commands.hashing.sha1 {
  public class GetFileSha1Hash {
    public static string Execute(string path) {
      using (var fs = new FileStream(path, FileMode.Open)) {
        using (var bs = new BufferedStream(fs)) {
          using (var sha1 = new SHA1Managed()) {
            var ba = sha1.ComputeHash(bs);
            var hexStringHash = Convert.ToHexString(ba);
            return hexStringHash;
          }
        }
      }
    }
  }
}


//public static string ExecuteOld(string path) {
//  using (var fs = new FileStream(path, FileMode.Open)) {
//    using (var bs = new BufferedStream(fs)) {
//      using (var sha1 = new SHA1Managed()) {
//        var ba = sha1.ComputeHash(bs);
//        var sb = new StringBuilder(2 * ba.Length);
//        foreach (var b in ba) {
//          sb.AppendFormat("{0:X2}", b);
//        }
//        return sb.ToString();
//      }
//    }
//  }
//}