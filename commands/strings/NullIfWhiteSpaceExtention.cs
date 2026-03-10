namespace CoarUtils.commands.strings {
  public static class NullIfWhiteSpaceExtention {
    public static string Execute(string s) {
      return string.IsNullOrWhiteSpace(s) ? null : s;
    }
    public static string NullIfWhiteSpace(this string s) {
      return string.IsNullOrWhiteSpace(s) ? null : s;
    }
  }
}
