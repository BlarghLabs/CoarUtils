using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoarUtils.commands.csv {
  public class CreateCSVTextFile {
    public static string Execute<T>(List<T> data, bool addHeaderRow = false) {
      var properties = typeof(T).GetProperties();
      var result = new StringBuilder();

      if (addHeaderRow) {
        var methods = properties
                    .Select(p => p.GetMethod)
                    .Select(v => StringToCSVCell(
                      ((v == null) ? "" : v.ToString())
                    ))
                    .Select(m => m.Substring(m.IndexOf("get_") + 4).Replace("()", ""));
        result.AppendLine(string.Join(",", methods));
      }

      foreach (var row in data) {
        var values = properties.Select(p => p.GetValue(row, null))
                               .Select(v => StringToCSVCell(
                                ((v == null) ? "" : v.ToString())
                                ));
        var line = string.Join(",", values);
        result.AppendLine(line);
      }

      return result.ToString();
    }

    private static string StringToCSVCell(string str) {
      bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
      if (mustQuote) {
        var sb = new StringBuilder();
        sb.Append("\"");
        foreach (char nextChar in str) {
          sb.Append(nextChar);
          if (nextChar == '"')
            sb.Append("\"");
        }
        sb.Append("\"");
        return sb.ToString();
      }

      return str;
    }

  }
}