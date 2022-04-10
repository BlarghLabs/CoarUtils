
namespace CoarUtils.commands.conversions.speed {
  public class MilesPerHourToMetersPerSecond {
    public static decimal Execute(decimal milesPerHour) {
      //https://www.inchcalculator.com/convert/mile-per-hour-to-meter-per-second/
      var metersPerSecond = milesPerHour * 0.44704M;
      return metersPerSecond;
    }
  }
}


