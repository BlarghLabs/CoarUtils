namespace CoarUtils.commands.conversions.length {
  public class InchesToCentimeters {

    public static decimal? Execute(
      decimal? inches
    ) {
      if (!inches.HasValue) {
        return null;
      }
      var response = inches.Value * 2.54M;
      return response;
    }

  }
}