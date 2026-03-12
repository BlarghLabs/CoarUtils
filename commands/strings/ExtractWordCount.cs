namespace CoarUtils.commands.strings {
  public static class ExtractWordCount {
    public static int Execute(this string input) {
      if (string.IsNullOrWhiteSpace(input)) {
        return 0;
      }
      var words = input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
      return words.Length;
    }
  }
}
