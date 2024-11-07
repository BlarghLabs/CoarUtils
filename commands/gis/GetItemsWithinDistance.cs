namespace CoarUtils.commands.gis {
  //credit: https://github.com/BlarghLabs/MoarUtils/blob/master/commands/gis/DistCalc.cs

  public static class GetItemsWithinDistance {
    public class Request {
      public List<Coordinate> loc { get; set; }
      public Coordinate c { get; set; }
      public double allowableDistanceInMiles { get; set; }
      public int? limitResultsToClosestX { get; set; }
    }

    public class Coordinate {
      public double lat { get; set; }
      public double lng { get; set; }
      public double distanceToPoint { get; set; }
    }


    //Geocoder.us
    //http://geocoder.us/help/utility.shtml
    //http://geocoder.us/service/distance?lat1=38&lat2=39&lng1=-122&lng2=-123  
    //15 seconds throtttle
    //http://username:password@geocoder.us/member/service/distance?lat1=38&lat2=39&lng1=-122&lng2=-123
    //http://geocoder.us/member/account
    //$50 for 20,000 queries

    //Some French Site
    //http://www.lacosmo.com/ortho/ortho.html

    public static List<Coordinate> Execute(
      Request request
    ) {
      //only limit results if passed
      if (!request.limitResultsToClosestX.HasValue) {
        request.limitResultsToClosestX = request.loc.Count;
      }

      var results = new List<Coordinate> { };
      for (int i = 0; i < request.loc.Count; i++) {
        var c1 = request.loc[i];
        c1.distanceToPoint = CalculateDistance.Execute(request: new CalculateDistance.Request {
          latA = request.c.lat,
          lngA = request.c.lng,
          latB = c1.lat,
          lngB = c1.lng,
          measurement = CalculateDistance.Measurement.miles,
        });
        if (c1.distanceToPoint <= request.allowableDistanceInMiles) {
          results.Add(c1);
        }
      }
      results = results
        .OrderBy(x => x.distanceToPoint)
        .ToList()
      ;
      return results;
    }
  }
}