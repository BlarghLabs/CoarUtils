using System.Net;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;

namespace CoarUtils.commands.gis {
  /// <summary>
  /// TARGET command shape - see CLAUDE.md "Legacy -> Target Migration". Converted 2026-09-02.
  /// </summary>
  public static class GetGeographicMidpoint {
    #region models
    public class Coordinate {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
    }
    public class Request {
      public List<Coordinate> coordinates { get; set; } = new List<Coordinate> { };
    }
    public class Response : ResponseStatusModel {
      public Coordinate coordinate { get; set; } = new Coordinate { };
    }
    #endregion

    // Not async: this command does no I/O. Declaring Task<Response> without `async` is the
    // honest way to satisfy the target command shape - there is no fake await to silence a
    // warning, and no second method pretending to be the real one. When real async work
    // arrives, add `async` and drop the Task.FromResult wrappers.
    public static Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };
      try {
        if (request == null) {
          return Task.FromResult(response = new Response { status = "params not found" });
        }
        if (request.coordinates == null || request.coordinates.Count == 0) {
          return Task.FromResult(response = new Response { status = "no coordinates provided" });
        }
        if (request.coordinates.Count == 1) {
          // A success return sets the fields on the existing response - building a new one here
          // would discard the coordinate just assigned. See CLAUDE.md "Success Returns Must Return
          // the Response They Filled In".
          response.coordinate = request.coordinates.Single();
          response.status = "only one provided";
          response.httpStatusCode = HttpStatusCode.OK;
          return Task.FromResult(response);
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

        response.httpStatusCode = HttpStatusCode.OK;
        return Task.FromResult(response);
      } catch (OperationCanceledException) {
        return Task.FromResult(response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS });
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return Task.FromResult(response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS });
        }
        LogIt.E(ex, cancellationToken);
        return Task.FromResult(response = new Response {
          httpStatusCode = HttpStatusCode.InternalServerError,
          status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS,
        });
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          request,
          response.coordinate,
        }, Formatting.None), cancellationToken);
      }
    }
  }
}
