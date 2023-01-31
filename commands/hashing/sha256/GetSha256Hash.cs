using System.Security.Cryptography;
using System.Text;

namespace CoarUtils.commands.hashing.sha256 {
  public static class GetSha256Hash {

    //public static string Execute(string value) {
    //  var sb = new StringBuilder();
    //  using (var hash = SHA256.Create()) {
    //    var enc = Encoding.UTF8;
    //    var ba = hash.ComputeHash(enc.GetBytes(value));
    //    foreach (var b in ba)
    //      sb.Append(b.ToString("x2"));
    //  }
    //  var base64 = Base64Encode.Execute(sb.ToString());
    //  return base64;
    //}

    //for later to add salt: https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp
    //https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/
    public static string Execute(string data) {
      // Create a SHA256   
      using (SHA256 sha256Hash = SHA256.Create()) {
        // ComputeHash - returns byte array  
        var ba = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

        // Convert byte array to a string   
        var sb = new StringBuilder();
        for (int i = 0; i < ba.Length; i++) {
          sb.Append(ba[i].ToString("x2"));
        }
        return sb.ToString();
      }
    }

    //TODO: ind validate this, then add for byte array
  }
}

//public static string Execute(byte[] ba) {
//  string hexStringHash;
//  using (var sha1 = new SHA1CryptoServiceProvider()) {
//    hexStringHash = Convert.ToHexString(sha1.ComputeHash(ba));
//  }
//  return hexStringHash;
//}

//public static string Execute(string input) {
//  var ba = Encoding.UTF8.GetBytes(input);
//  return Execute(ba);
//}