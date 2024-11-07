using System.Security.Cryptography;
using System.Text;

namespace CoarUtils.commands.cryptography {
  public static class StringCyptography {
    /// <summary>
    /// Create and initialize a crypto algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    private static SymmetricAlgorithm GetAlgorithm(string secret) {
      var algorithm = Rijndael.Create();
      var rdb = new Rfc2898DeriveBytes(secret, new byte[] {
        0x53,0x6f,0x64,0x69,0x75,0x6d,0x20,             // salty goodness
        0x43,0x68,0x6c,0x6f,0x72,0x69,0x64,0x65
    });
      algorithm.Padding = PaddingMode.ISO10126;
      algorithm.Key = rdb.GetBytes(32);
      algorithm.IV = rdb.GetBytes(16);
      return algorithm;
    }


    /// <summary>
    /// Encrypts a string with a given secret.
    /// </summary>
    /// <param name="clearText">The clear text.</param>
    /// <param name="secret">The secret.</param>
    public static string Encrypt(string clearText, string secret) {
      var algorithm = GetAlgorithm(secret);
      var encryptor = algorithm.CreateEncryptor();
      var clearBytes = Encoding.UTF8.GetBytes(clearText);
      using (var ms = new MemoryStream())
      using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
        cs.Write(clearBytes, 0, clearBytes.Length);
        cs.Close();
        return Convert.ToBase64String(ms.ToArray());
      }
    }

    public static string EncryptBase64Encoded(string cipherText, string secret) {
      var ba = Encoding.UTF8.GetBytes(cipherText);
      var unexcryptedBase64 = Convert.ToBase64String(ba);
      var encrypted = Encrypt(unexcryptedBase64, secret);
      return encrypted;
    }

    /// <summary>
    /// Decrypts a string using a given secret.
    /// </summary>
    /// <param name="cipherText">The cipher text.</param>
    /// <param name="secret">The secret.</param>
    public static string Decrypt(string cipherText, string secret) {
      var algorithm = GetAlgorithm(secret);
      var decryptor = algorithm.CreateDecryptor();
      var cipherBytes = Convert.FromBase64String(cipherText);
      using (var ms = new MemoryStream())
      using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write)) {
        cs.Write(cipherBytes, 0, cipherBytes.Length);
        cs.Close();
        return Encoding.UTF8.GetString(ms.ToArray());
      }
    }

    public static string DecryptBase64Encoded(string cipherText, string secret) {
      var decrypted = Decrypt(cipherText, secret);
      var decryptedBa = Convert.FromBase64String(decrypted);
      var decryptedString = Encoding.UTF8.GetString(decryptedBa);
      return decryptedString;
    }
  }
}
