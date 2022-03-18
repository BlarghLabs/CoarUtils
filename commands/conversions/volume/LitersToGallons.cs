using System;

namespace CoarUtils.commands.conversions.volume {
  public class LitersToGallons {

    public static decimal? Execute(
      decimal? liters,
      bool roundToZeroDecimals = false
    ) {
      if (!liters.HasValue) {
        return null;
      }

      //https://www.inchcalculator.com/convert/gallon-to-liter/#:~:text=To%20convert%20a%20gallon%20measurement,volume%20by%20the%20conversion%20ratio.&text=The%20volume%20in%20liters%20is%20equal%20to%20the%20gallons%20multiplied%20by%203.785412.
      //var result = liters.Value / 3.785412M;

      //http://www.formulaconversion.com/formulaconversioncalculator.php?convert=cubicfeet_to_cubicmeters
      //var result = liters.Value / 3.78541178M;

      //https://www.asknumbers.com/liters-to-gallons.aspx
      var result = liters.Value / 3.785411784M;

      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }
  }
}
