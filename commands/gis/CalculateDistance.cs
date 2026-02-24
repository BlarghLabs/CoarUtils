namespace CoarUtils.commands.gis {
  //credit: https://github.com/BlarghLabs/MoarUtils/blob/master/commands/gis/DistCalc.cs
  public static class CalculateDistance {
    #region models
    public class Request {
      public double latA { get; set; }
      public double lngA { get; set; }
      public double latB { get; set; }
      public double lngB { get; set; }
      public Measurement measurement = Measurement.miles;

    }
    #endregion

    public enum Measurement {
      miles = 0,
      kilometers = 1
    }
    public const double EarthRadiusInMiles = 3956.0;
    public const double EarthRadiusInKilometers = 6367.0;
    public static double ToRadian(double val) { return val * (Math.PI / 180); }
    public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }



    /// <summary>
    /// Calculate the distance between two geocodes. Defaults to using Miles.
    /// </summary> 
    public static double Execute(
      Request request
    ) {
      var radius = EarthRadiusInMiles;
      if (request.measurement == Measurement.kilometers) {
        radius = EarthRadiusInKilometers;
      }
      var dist = radius
        * 2
        * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(request.latA, request.latB)) / 2.0), 2.0) + Math.Cos(ToRadian(request.latA)) * Math.Cos(ToRadian(request.latB)) * Math.Pow(Math.Sin((DiffRadian(request.lngA, request.lngB)) / 2.0), 2.0)))))
      ;
      return dist;
    }
  }
}