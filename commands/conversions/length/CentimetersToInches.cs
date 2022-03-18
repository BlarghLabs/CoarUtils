using System;

namespace CoarUtils.commands.conversions.length {
  public class CentimetersToInches {
    public static decimal? Execute(
      decimal? cm,
      bool roundToZeroDecimals = false
    ) {
      if (!cm.HasValue) {
        return null;
      }
      var result = cm.Value / 2.54M;
      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }
  }
}