using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.ipgeolocation {

  /// <summary>
  /// TARGET command shape - see CLAUDE.md "Legacy -> Target Migration". Converted 2026-09-02.
  ///
  /// Three things the conversion fixed beyond the signature:
  ///
  ///   client.City() is a NETWORK CALL to MaxMind and was synchronous, blocking a request thread
  ///     for the round trip. It is CityAsync now.
  ///   the catch arm returned the literal "task cancelled", a third spelling of a status that is
  ///     "cancellation requested" everywhere else - so a log query or metric filter written against
  ///     the standard wording could never match it.
  ///   Response did not inherit ResponseStatusModel, so it carried no status at all.
  /// </summary>
  public static class GetGeolocation {
    #region models
    public class Request {
      public string ip { get; set; }
      public int maxmindAccountId { get; set; }
      public string maxmindAccountKey { get; set; }
    }

    public class Response : ResponseStatusModel {
      public CityResponse cr { get; set; }
    }
    #endregion

    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };
      try {
        #region validation
        if (request == null) {
          return response = new Response { status = "params not found" };
        }
        if (string.IsNullOrEmpty(request.ip)) {
          return response = new Response { status = "ip not found" };
        }
        var localhosts = new List<string> {
          "127.0.0.1",
          "localhost",
          "::1",
        };
        if (localhosts.Contains(request.ip)) {
          return response = new Response { status = "ip is localhost" };
        }
        #endregion

        using (var client = new WebServiceClient(request.maxmindAccountId, request.maxmindAccountKey)) {
          response.cr = await client.CityAsync(request.ip);

          LogIt.I(JsonConvert.SerializeObject(new {
            //most common
            countryIsoCode = response.cr.Country.IsoCode, // 'US'
            countryName = response.cr.Country.Name,  // 'United States'

            mostSpecificSubdivisionName = response.cr.MostSpecificSubdivision.Name, // 'Minnesota'
            MostSpecificSubdivisionIsoCode = response.cr.MostSpecificSubdivision.IsoCode, // 'MN'

            cityName = response.cr.City.Name, // 'Minneapolis'

            postalCode = response.cr.Postal.Code, // '55455'

            lat = response.cr.Location.Latitude,  // 44.9733
            lng = response.cr.Location.Longitude, // -93.2323
          }, Formatting.None), cancellationToken);
        }

        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (OperationCanceledException) {
        return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          return response = new Response { status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS };
        }
        LogIt.E(ex, cancellationToken);
        return response = new Response {
          httpStatusCode = HttpStatusCode.InternalServerError,
          status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS,
        };
      } finally {
        // The account key is a credential and must never reach a log.
        if (request != null) {
          request.maxmindAccountKey = "DO_NOT_LOG";
          request.maxmindAccountId = -1;
        }

        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          request,
        }, Formatting.None), cancellationToken);
      }
    }
  }
}
