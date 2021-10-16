using System;
using System.Text.RegularExpressions;

namespace CoarUtils.commands.strings {
  //https://stackoverflow.com/questions/18153998/how-do-i-remove-all-html-tags-from-a-string-without-knowing-which-tags-are-in-it
  public class StripHtmlTags {
    public static string Execute(string input, string replaceWith = "") {
      var result = Regex.Replace(input, "<.*?>", replaceWith);
      return result;
    }
  }
}

