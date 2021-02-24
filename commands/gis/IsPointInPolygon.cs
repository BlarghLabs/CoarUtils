using System.Collections.Generic;

namespace CoarUtils.commands.gis {
  //credit: https://github.com/BlarghLabs/MoarUtils/blob/master/commands/gis/DistCalc.cs
  public static class IsPointInPolygon {
    public class Polygon {
      public Polygon() {
        loc = new List<Coordinate> { };
        exclusionaryPolygons = new List<Polygon> { };
      }
      public List<Coordinate> loc { get; set; }
      public List<Polygon> exclusionaryPolygons { get; set; }
    }

    public class Coordinate {
      public double lat { get; set; }
      public double lng { get; set; }
      public double distanceToPoint { get; set; }
    }


    public static bool Execute(Coordinate c, List<Polygon> lop) {
      foreach (var p in lop) {
        if (Execute(p, c)) {
          return true;
        }
      }
      return false;
    }

    public static bool Execute(Coordinate c, Polygon p) {
      if (!Execute(p.loc, c)) {
        //was not in priary poly
        return false;
      } else {
        //If in outer poly, then confirm not in inner poly
        foreach (var ep in p.exclusionaryPolygons) {
          if (Execute(ep.loc, c)) {
            //was in both primary but also exclusionary
            return false;
          }
        }
        //was in primary but not exclusonary
        return true;
      }
    }

    public static bool Execute(List<Coordinate> loc, Coordinate c) {
      var result = false;

      //Minus 1 bc 1 pt is listed twice (could otherwsie accomplish w/ give me distnct)
      int iNumberOfVerticies = (loc.Count == 0) ? /* no poly creted yet */ 0 : loc.Count - 1;

      //Do Work
      int i, j;
      for (i = 0, j = iNumberOfVerticies - 1; i < iNumberOfVerticies; j = i++) {
        if (((loc[i].lat > c.lat) != (loc[j].lat > c.lat))
          && (c.lng < (loc[j].lng - loc[i].lng) * (c.lat - loc[i].lat) / (loc[j].lat - loc[i].lat) + loc[i].lng)) {
          result = !result;
        }
      }

      return result;
    }
  }

}
}