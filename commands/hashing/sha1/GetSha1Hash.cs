using System;
using System.Security.Cryptography;

namespace CoarUtils.commands.hashing.sha1 {
  public class GetSha1Hash {
    public static string Execute(byte[] ba) {
      string hash;
      using (var sha1 = new SHA1CryptoServiceProvider()) {
        hash = Convert.ToBase64String(sha1.ComputeHash(ba));
      }
      return hash;
    }
  }
}

