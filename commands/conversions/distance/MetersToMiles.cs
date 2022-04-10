namespace CoarUtils.commands.conversions.distance {
  public class MetersToMiles {
    public static decimal Execute(decimal meters) {
      var miles = meters * 0.000621371192M;
      return miles;
    }
  }
}
