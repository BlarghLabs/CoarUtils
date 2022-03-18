namespace CoarUtils.commands.conversions.length {
  public class MilesToMeters {
    public static decimal Execute(decimal miles) {
      var meters = miles * 1609.344M;
      return meters;
    }
  }
}


