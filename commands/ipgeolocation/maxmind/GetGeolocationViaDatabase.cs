using System.Net;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;

namespace CoarUtils.commands.ipgeolocation.maxmind {

  public static class GetGeolocationViaDatabase {
    #region models
    public class Request {
      public string ip { get; set; }
      public string databasePath { get; set; } = "GeoIP2-City.mmdb";
    }

    public class Response : ResponseStatusModel {
      public CityResponse cityResponse { get; set; }
    }
    #endregion

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };

      try {
        #region validation
        if (string.IsNullOrEmpty(request.ip)) {
          return response = new Response { status = $"ip not found" };
        }
        var localhosts = new List<string> {
          "127.0.0.1",
          "localhost",
          "::1",
        };
        if (localhosts.Contains(request.ip)) {
          return response = new Response { status = $"ip is localhost" };
        }
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        #endregion

        // This creates the DatabaseReader object, which should be reused across
        // lookups.
        using (var reader = new DatabaseReader(request.databasePath)) {
          if (!reader.TryCity(request.ip, out var cityResponse)) {
            return response = new Response { status = $"unable to geolocate" };
          }
          response.cityResponse = cityResponse;

          LogIt.I(JsonConvert.SerializeObject(new {
            //response.cityResponse,
            //most common
            //response.cityResponse.Country.Names["zh-CN"]); // '美国'
            countryIsoCode = response.cityResponse?.Country?.IsoCode, // 'US'
            countryName = response.cityResponse?.Country?.Name,  // 'United States'
            mostSpecificSubdivisionName = response.cityResponse?.MostSpecificSubdivision?.Name, // 'Minnesota'
            MostSpecificSubdivisionIsoCode = response.cityResponse?.MostSpecificSubdivision?.IsoCode, // 'MN'
            cityName = response.cityResponse?.City?.Name, // 'Minneapolis'
            postalCode = response.cityResponse?.Postal?.Code, // '55455'
            lat = response.cityResponse?.Location?.Latitude,  // 44.9733
            lng = response.cityResponse?.Location?.Longitude, // -93.2323
          }, Formatting.Indented), cancellationToken);
        }

        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new Response { status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS, httpStatusCode = HttpStatusCode.InternalServerError };
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          //response.cityResponse,
          countryIsoCode = response.cityResponse?.Country?.IsoCode, // 'US'
          countryName = response.cityResponse?.Country?.Name,  // 'United States'
          mostSpecificSubdivisionName = response.cityResponse?.MostSpecificSubdivision?.Name, // 'Minnesota'
          MostSpecificSubdivisionIsoCode = response.cityResponse?.MostSpecificSubdivision?.IsoCode, // 'MN'
          cityName = response.cityResponse?.City?.Name, // 'Minneapolis'
          postalCode = response.cityResponse?.Postal?.Code, // '55455'
          lat = response.cityResponse?.Location?.Latitude,  // 44.9733
          lng = response.cityResponse?.Location?.Longitude, // -93.2323
          request,
        }, Formatting.Indented), cancellationToken);
      }
    }
  }
}
