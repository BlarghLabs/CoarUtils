using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CoarUtils.commands.hashing.sha1 {
  public class GetSha1Hash {
    public static string Execute(byte[] ba) {
      string hash;
      using (var sha1 = new SHA1CryptoServiceProvider()) {
        hash = Convert.ToBase64String(sha1.ComputeHash(ba));
      }
      return hash;
    }

    //public static string GetHash(string input) {
    //  return string.Join("", (new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input))).Select(x => x.ToString("X2")).ToArray());
    //}
    public static string Execute(string input) {
      using (var sha1 = new SHA1CryptoServiceProvider()) {
        var hash = string.Join(
          "",
          sha1.ComputeHash(Encoding.UTF8.GetBytes(input))
            .Select(x => x.ToString("X2"))
            .ToArray()
        );
        return hash;
      }
    }
  }
}

