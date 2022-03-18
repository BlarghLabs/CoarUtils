namespace CoarUtils.commands.conversions.volume {
  public class CubicFeetToCubicMeters {
    //http://www.formulaconversion.com/formulaconversioncalculator.php?convert=cubicfeet_to_cubicmeters
    public static decimal? Execute(decimal? cubicFeet) {
      if (!cubicFeet.HasValue) {
        return null;
      }
      //was: var result = cubicFeet.Value / 35.31466672M;
      //http://www.formulaconversion.com/formulaconversioncalculator.php?convert=cubicfeet_to_cubicmeters
      //http://cashmancuneo.net/paper/metriconv.pdf
      var result = cubicFeet.Value * 0.02831684659M;
      return result;
    }
  }
}