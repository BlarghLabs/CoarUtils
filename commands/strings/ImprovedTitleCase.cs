using System.Globalization;
using System.Text.RegularExpressions;

namespace CoarUtils.commands.strings {
  
  public static class ImprovedTitleCase {
    /// <summary>
    /// this mitigates apostrophe s being 'S
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public static string Execute(this string input) {
      input = input?.Trim();
      if (string.IsNullOrWhiteSpace(input))
        return input;

      // Use the standard ToTitleCase method
      TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
      string titleCased = textInfo.ToTitleCase(input.ToLower());

      // Fix possessives
      titleCased = Regex.Replace(titleCased, "('S\\b)", "'s");

      // Find and preserve acronyms including possessives
      string[] words = input.Split(' ');
      string[] titleCasedWords = titleCased.Split(' ');

      for (int i = 0; i < words.Length && i < titleCasedWords.Length; i++) {
        string originalWord = words[i];
        // Check if word is an acronym (2+ uppercase letters)
        if (Regex.IsMatch(originalWord, @"\b[A-Z]{2,}")) {
          // Handle possessive case
          if (originalWord.EndsWith("'s", StringComparison.Ordinal) ||
              originalWord.EndsWith("'S", StringComparison.Ordinal)) {
            // Keep the acronym uppercase but ensure possessive 's is lowercase
            string acronymBase = originalWord.Substring(0, originalWord.Length - 2);
            titleCasedWords[i] = acronymBase + "'s";
          } else {
            // Just a regular acronym, preserve it as-is
            titleCasedWords[i] = originalWord;
          }
        }
      }

      return string.Join(" ", titleCasedWords);
    }
  }
}
