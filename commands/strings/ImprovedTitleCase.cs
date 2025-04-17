using System.Globalization;
using System.Text.RegularExpressions;

namespace CoarUtils.commands.strings {
  
  public static class ImprovedTitleCase {
    /// <summary>
    /// this mitigates apostrophe s being 'S
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public static string Execute(this string input) {
      if (string.IsNullOrWhiteSpace(input))
        return input;

      // Use the standard ToTitleCase method first
      var textInfo = CultureInfo.CurrentCulture.TextInfo;
      var titleCased = textInfo.ToTitleCase(input.ToLower());

      // Fix apostrophe + s issues by finding patterns like "'S" and converting to "'s"
      titleCased = Regex.Replace(titleCased, "('S\\b)", "'s");

      return titleCased;
    }
  }
}
