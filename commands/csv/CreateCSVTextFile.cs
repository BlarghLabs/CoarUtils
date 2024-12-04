using System.Text;
using CoarUtils.commands.logging;
namespace CoarUtils.commands.csv {
  public class CreateCSVTextFile {
    public static string Execute<T>(List<T> data, bool addHeaderRow = false) {
      try {
        var properties = typeof(T).GetProperties();
        var result = new StringBuilder();

        if (addHeaderRow) {
          var methods = properties
                      .Select(x => x.GetMethod)
                      .Select(x => StringToCSVCell(
                        ((x == null) ? "" : x.ToString())
                      ))
                      .Select(x => x.Substring(x.IndexOf("get_") + 4).Replace("()", ""));
          result.AppendLine(string.Join(",", methods));
        }

        foreach (var row in data) {
          var values = properties.Select(x => x.GetValue(row, null))
                                 .Select(x => StringToCSVCell(
                                  ((x == null) ? "" : x.ToString())
                                  ));
          var line = string.Join(",", values);
          result.AppendLine(line);
        }

        return result.ToString();
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }

    private static string StringToCSVCell(string str) {
      try {
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
      } catch (Exception ex) {
        LogIt.E(ex);
        throw ex;
      }
    }

  }
}