namespace CoarUtils.commands.conversions.weight {
  public class PoundsToKilograms {

    public static decimal? Execute(
      decimal? pounds,
      bool roundToZeroDecimals = false
    ) {
      if (!pounds.HasValue) {
        return null;
      }

      //https://www.asknumbers.com/kilograms-to-ounces.aspx
      var result = pounds.Value / 2.2046226218M;

      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }
  }
}
