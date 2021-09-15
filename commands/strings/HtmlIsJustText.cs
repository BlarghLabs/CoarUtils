using System.Linq;
using HtmlAgilityPack;

namespace CoarUtils.commands.strings {
  public class HtmlIsJustText {
    public static bool Execute(HtmlNode rootNode) {
      return rootNode.Descendants().All(n => n.NodeType == HtmlNodeType.Text);
    }
  }
}
