using System.Text;

namespace CoarUtils.commands.base64 {
  public static class Base64Encode {
    public static string Execute(string plainText) {
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      return Convert.ToBase64String(plainTextBytes);
    }
  }
}
