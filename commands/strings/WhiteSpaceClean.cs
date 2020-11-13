using System.Text.RegularExpressions;
using System.Web;

namespace CoarUtils.commands.strings {
  public static class WhiteSpaceClean {
    public static string Execute(string orig) {
      if (string.IsNullOrWhiteSpace(orig)) {
        return "";
      }
      string result = Regex.Replace(HttpUtility.HtmlDecode(orig), @"\t|\n|\r", " ");
      result = Regex.Replace(result, @"\s+", " ").Trim();
      return result;
    }
  }
}
