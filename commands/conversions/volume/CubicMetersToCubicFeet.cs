﻿namespace CoarUtils.commands.conversions.volume {
  public class CubicMetersToCubicFeet {
    public static decimal? Execute(
      decimal? cubicMeters,
      bool roundToZeroDecimals = false
    ) {
      if (!cubicMeters.HasValue) {
        return null;
      }

      //was: var result = cubicMeters.Value * 35.31466672M;
      var result = cubicMeters.Value * 35.314670111696704M;

      //https://books.google.rs/books?id=RAxmewUGvf4C&pg=PA170&lpg=PA170&dq=0.02831684659&source=bl&ots=lc-4fbSxmt&sig=eR8-fpHHo646c1kf8JVmN6NRiws&hl=en&sa=X&ved=0ahUKEwiM6diaye_VAhUJ6xQKHRL1AbMQ6AEINjAE#v=onepage&q=0.028316846590.02831684659&f=false
      //http://www.formulaconversion.com/formulaconversioncalculator.php?convert=cubicfeet_to_cubicmeters
      //http://cashmancuneo.net/paper/metriconv.pdf
      //OR: var result = cubicMeters.Value / 0.02831684659M;

      return !roundToZeroDecimals ? result : Math.Round(result, 0);
    }
  }
}