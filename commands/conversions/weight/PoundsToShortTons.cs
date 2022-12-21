using System;

namespace CoarUtils.commands.conversions.weight {
  public class PoundsToShortTons {

    public static decimal? Execute(
      decimal? pounds
    ) {
      if (!pounds.HasValue) {
        return null;
      }

      //https://www.checkyourmath.com/convert/weight_mass/lb_short_ton.php
      var result = pounds.Value / 2000M;

      return result;
    }
  }
}
