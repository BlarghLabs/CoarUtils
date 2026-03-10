namespace CoarUtils.commands.strings {
  public static class NullIfWhiteSpace {
    public static string Execute(this string s) {
      return string.IsNullOrWhiteSpace(s) ? null : s;
    }
  }
}
