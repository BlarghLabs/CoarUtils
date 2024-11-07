using HtmlAgilityPack;

namespace CoarUtils.commands.strings {
  public class ContainsHTMLElements {
    public static bool Execute(string text) {
      var doc = new HtmlDocument();
      doc.LoadHtml(text);
      return !HtmlIsJustText.Execute(doc.DocumentNode);
    }
  }
}
