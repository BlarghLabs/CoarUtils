using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CoarUtils.commands.strings {
  public static class ImprovedTitleCase {
    /// <summary>
    /// Improves title casing by handling apostrophes correctly and preserving acronyms
    /// </summary>
    public static string Execute(this string input) {
      input = input?.Trim();
      if (string.IsNullOrWhiteSpace(input))
        return input;

      // Standardize apostrophes first (convert any apostrophe variants to standard straight apostrophe)
      input = NormalizeApostrophes(input);

      // Use the standard ToTitleCase method
      TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
      string titleCased = textInfo.ToTitleCase(input.ToLower());

      // Fix possessives with various apostrophe types
      titleCased = FixPossessives(titleCased);

      // Handle acronyms
      titleCased = PreserveAcronyms(input, titleCased);

      return titleCased;
    }

    private static string NormalizeApostrophes(string input) {
      // Replace various apostrophe characters with the standard straight apostrophe
      return input.Replace('\u2032', '\'')  // Prime
                 .Replace('\u2019', '\'')   // Right single quotation mark
                 .Replace('`', '\'')        // Backtick
                 .Replace('\u00B4', '\'');  // Acute accent
    }

    private static string FixPossessives(string input) {
      // Handle straight apostrophe and any other apostrophe variants that might remain
      return Regex.Replace(input, "('S\\b)", "'s");
    }

    private static string PreserveAcronyms(string original, string titleCased) {
      string[] originalWords = original.Split(' ');
      string[] titleCasedWords = titleCased.Split(' ');

      for (int i = 0; i < Math.Min(originalWords.Length, titleCasedWords.Length); i++) {
        // Check if the original word is an acronym
        if (Regex.IsMatch(originalWords[i], @"\b[A-Z]{2,}")) {
          string acronym = originalWords[i];

          // Check for possessive form with apostrophe
          if (acronym.EndsWith("'s", StringComparison.OrdinalIgnoreCase) ||
              acronym.EndsWith("'S", StringComparison.OrdinalIgnoreCase)) {
            // Keep acronym but ensure possessive is lowercase
            int apostrophePos = acronym.Length - 2;
            string acronymBase = acronym.Substring(0, apostrophePos);
            titleCasedWords[i] = acronymBase + "'s";
          } else {
            titleCasedWords[i] = acronym;
          }
        }

        // One final check for any remaining uppercase 'S after apostrophes
        if (titleCasedWords[i].Contains("'S")) {
          titleCasedWords[i] = titleCasedWords[i].Replace("'S", "'s");
        }
      }

      return string.Join(" ", titleCasedWords);
    }
  }
}