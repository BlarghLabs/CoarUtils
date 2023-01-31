using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CoarUtils.commands.gis {
  public static class GetGeographicMidpoint {
    #region models
    public class Coordinate {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
    }
    public class Request {
      public Request() {
        loc = new List<Coordinate> { };
      }
      public List<Coordinate> loc { get; set; }
    }
    public class respone {
      public respone() {
      }
      public Coordinate? c { get; set; }
    }
    #endregion


    public static void Execute(
      Request m,
      out respone r,
      out HttpStatusCode hsc,
      out string status
    ) {
      r = new respone {
        c = new Coordinate { },
      };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        //temp:
        //m.loc.Add(new Coordinate { lat = 22.9833M, lng = 72.5000M }); //Sarkhej
        //m.loc.Add(new Coordinate { lat = 18.9750M, lng = 72.8258M }); //Mumbai
        //m.loc.Add(new Coordinate { lat = 22.3000M, lng = 73.2003M }); //Vadodara
        //m.loc.Add(new Coordinate { lat = 26.9260M, lng = 75.8235M }); //Jaipur
        //m.loc.Add(new Coordinate { lat = 28.6100M, lng = 77.2300M }); //Delhi
        //m.loc.Add(new Coordinate { lat = 22.3000M, lng = 70.7833M }); //Rajkot

        if (m.loc == null || m.loc.Count == 0) {
          status = "no coordinates provided";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (m.loc.Count == 1) {
          r.c = m.loc.Single();
          hsc = HttpStatusCode.OK;
          status = "only one provided";
          return;
        }

        double x = 0, y = 0, z = 0;
        foreach (var _c in m.loc) {
          var latitude = Convert.ToDouble(_c.lat) * Math.PI / 180;
          var longitude = Convert.ToDouble(_c.lng) * Math.PI / 180;

          x += Math.Cos(latitude) * Math.Cos(longitude);
          y += Math.Cos(latitude) * Math.Sin(longitude);
          z += Math.Sin(latitude);
        }
        var total = m.loc.Count;
        x = x / total;
        y = y / total;
        z = z / total;
        var centralLongitude = Math.Atan2(y, x);
        var centralSquareRoot = Math.Sqrt(x * x + y * y);
        var centralLatitude = Math.Atan2(z, centralSquareRoot);
        r.c = new Coordinate {
          lat = ((decimal)centralLatitude * 180 / (decimal)Math.PI),
          lng = ((decimal)centralLongitude * 180 / (decimal)Math.PI)
        };

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexpected error";
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          m,
          r.c
        }));
      }
    }
  }
}
