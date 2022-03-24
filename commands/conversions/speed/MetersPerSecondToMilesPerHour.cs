namespace CoarUtils.commands.conversions.speed {
  public class MetersPerSecondToMilesPerHour {
    public static decimal Execute(decimal metersPerSecond) {
      //
      var milesPerHour = metersPerSecond * 2.236936M;
      return milesPerHour;
    }
  }
}
