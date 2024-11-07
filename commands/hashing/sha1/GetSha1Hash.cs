using System.Security.Cryptography;
using System.Text;

namespace CoarUtils.commands.hashing.sha1 {
  public class GetSha1Hash {
    //this is converting 160 byte sha1 array to 40 char hex
    public static string Execute(byte[] ba) {
      string hexStringHash;
      using (var sha1 = new SHA1CryptoServiceProvider()) {
        hexStringHash = Convert.ToHexString(sha1.ComputeHash(ba));
      }
      return hexStringHash;
    }

    public static string Execute(string input) {
      var ba = Encoding.UTF8.GetBytes(input);
      return Execute(ba);
    }
  }
}


//public static string GetHash(string input) {
//  return string.Join("", (new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input))).Select(x => x.ToString("X2")).ToArray());
//}

//this was outputting base64 encoded sha1 byte array 
//public static string ExecuteOld(byte[] ba) {
//  string hash;
//  using (var sha1 = new SHA1CryptoServiceProvider()) {
//    hash = Convert.ToBase64String(sha1.ComputeHash(ba));
//  }
//  return hash;
//}

//this is old method, have made less complicated in version above
//public static string Execute(string input) {
//  using (var sha1 = new SHA1CryptoServiceProvider()) {
//    var ba = Encoding.UTF8.GetBytes(input);
//    var hash = string.Join(
//      "",
//      sha1.ComputeHash(ba)
//        .Select(x => x.ToString("X2"))
//        .ToArray()
//    );
//    return hash;
//  }
//}