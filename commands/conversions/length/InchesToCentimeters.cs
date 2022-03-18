namespace CoarUtils.commands.conversions.length {
  public class InchesToCentimeters {

    public static decimal? Execute(
      decimal? inches
    ) {
      if (!inches.HasValue) {
        return null;
      }
      var result = inches.Value * 2.54M;
      return result;
    }

  }
}