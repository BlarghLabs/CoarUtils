namespace CoarUtils.commands.strings {
  public static class StringExtensions {
    public static string NullIfWhiteSpace(this string s) {
      return string.IsNullOrWhiteSpace(s) ? null : s;
    }
  }
}
