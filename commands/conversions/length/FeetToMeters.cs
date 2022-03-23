
namespace CoarUtils.commands.conversions.length {
  public class FeetToMeters {
    public static decimal Execute(decimal feet) {
      //https://www.sfei.org/it/gis/map-interpretation/conversion-constants
      var meters = feet * 0.304800609601M;
      return meters;
    }
  }
}


