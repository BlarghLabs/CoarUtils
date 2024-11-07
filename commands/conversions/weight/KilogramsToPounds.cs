namespace CoarUtils.commands.conversions.weight {
  public class KilogramsToPounds {

    public static decimal? Execute(
      decimal? kilograms,
      bool roundToZeroDecimals = false
    ) {
      if (!kilograms.HasValue) {
        return null;
      }

      //https://www.asknumbers.com/kilograms-to-ounces.aspx
      var result = kilograms.Value * 2.2046226218M;

      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }
  }
}
