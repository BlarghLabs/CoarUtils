using System;

namespace CoarUtils.commands.strings {
  public class StripNonNumericCharacters {
    public static string Execute(string number) {
      string numbersOnly = "";
      for (int i = 0; i < number.Length; i++) {
        numbersOnly += Char.IsNumber(number[i]) ? number[i].ToString() : "";
      }
      return numbersOnly;
    }
  }
}

