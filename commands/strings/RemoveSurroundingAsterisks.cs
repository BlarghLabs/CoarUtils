namespace CoarUtils.commands.strings {
  public static class RemoveSurroundingAsterisks {
    public static string Execute(this string input) {
      input = input?.Trim();
      if (string.IsNullOrWhiteSpace(input))
        return input;

      // Check if string starts with one or more asterisks and ends with one or more asterisks
      if (input.StartsWith("*") && input.EndsWith("*")) {
        // Count leading asterisks
        int leadingCount = 0;
        while (leadingCount < input.Length && input[leadingCount] == '*') {
          leadingCount++;
        }

        // Count trailing asterisks
        int trailingCount = 0;
        while (trailingCount < input.Length - leadingCount &&
               input[input.Length - 1 - trailingCount] == '*') {
          trailingCount++;
        }

        // Only trim if both ends have asterisks
        if (leadingCount > 0 && trailingCount > 0) {
          // Remove asterisks and trim any resulting whitespace
          return input.Substring(leadingCount, input.Length - leadingCount - trailingCount).Trim();
        }
      }

      return input;
    }

  }
}
