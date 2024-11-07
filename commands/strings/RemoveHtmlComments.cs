using System.Text.RegularExpressions;

namespace CoarUtils.commands.strings {
  public class RemoveHtmlComments {
    public static string Execute(string orig) {
      return Regex.Replace(orig, "<!--.*?-->", String.Empty, RegexOptions.Singleline);
    }

  }
}
