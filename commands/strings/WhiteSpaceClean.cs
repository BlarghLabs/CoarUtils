using System.Text.RegularExpressions;
using System.Web;

namespace CoarUtils.commands.strings {
  public static class WhiteSpaceClean {
    public static string Execute(string orig) {
      if (string.IsNullOrWhiteSpace(orig)) {
        return "";
      }
      string response = Regex.Replace(HttpUtility.HtmlDecode(orig), @"\t|\n|\r", " ");
      response = Regex.Replace(response, @"\s+", " ").Trim();
      return response;
    }
  }
}
