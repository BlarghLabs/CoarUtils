namespace CoarUtils.commands.strings {
  public class CondenseWhiteSpace {
    public static string Execute(string input) {
      string result = input;
      while (result.IndexOf("  ") != -1) {
        result = result.Replace("  ", " ");
      }
      return result;
    }
  }
}
