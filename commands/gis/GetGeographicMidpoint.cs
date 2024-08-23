using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.gis
{
    public static class GetGeographicMidpoint {
    #region models
    public class Coordinate {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
    }
    public class Request {
      public List<Coordinate> coordinates { get; set; } = new List<Coordinate> { };
    }
    public class Response : models.commands.ResponseStatusModel {
      public Coordinate? coordinate { get; set; } = new Coordinate { };
    }
    #endregion


    public static void Execute(
      Request request,
      out Response response,
      out HttpStatusCode hsc,
      out string status,
      CancellationToken cancellationToken
    ) {
      response = new Response {};
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        //temp:
        //request.loc.Add(new Coordinate { lat = 22.9833M, lng = 72.5000M }); //Sarkhej
        //request.loc.Add(new Coordinate { lat = 18.9750M, lng = 72.8258M }); //Mumbai
        //request.loc.Add(new Coordinate { lat = 22.3000M, lng = 73.2003M }); //Vadodara
        //request.loc.Add(new Coordinate { lat = 26.9260M, lng = 75.8235M }); //Jaipur
        //request.loc.Add(new Coordinate { lat = 28.6100M, lng = 77.2300M }); //Delhi
        //request.loc.Add(new Coordinate { lat = 22.3000M, lng = 70.7833M }); //Rajkot

        if (request.coordinates == null || request.coordinates.Count == 0) {
          status = "no coordinates provided";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        if (request.coordinates.Count == 1) {
          response.coordinate = request.coordinates.Single();
          hsc = HttpStatusCode.OK;
          status = "only one provided";
          return;
        }

        double x = 0, y = 0, z = 0;
        foreach (var coordinate in request.coordinates) {
          var latitude = Convert.ToDouble(coordinate.lat) * Math.PI / 180;
          var longitude = Convert.ToDouble(coordinate.lng) * Math.PI / 180;

          x += Math.Cos(latitude) * Math.Cos(longitude);
          y += Math.Cos(latitude) * Math.Sin(longitude);
          z += Math.Sin(latitude);
        }
        var total = request.coordinates.Count;
        x = x / total;
        y = y / total;
        z = z / total;
        var centralLongitude = Math.Atan2(y, x);
        var centralSquareRoot = Math.Sqrt(x * x + y * y);
        var centralLatitude = Math.Atan2(z, centralSquareRoot);
        response.coordinate = new Coordinate {
          lat = ((decimal)centralLatitude * 180 / (decimal)Math.PI),
          lng = ((decimal)centralLongitude * 180 / (decimal)Math.PI)
        };

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          request,
          response.coordinate
        }));
      }
    }
  }
}
