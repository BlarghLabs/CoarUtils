﻿using System.Net;
using CoarUtils.commands.logging;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;

namespace CoarUtils.commands.ipgeolocation.maxmind {

  public static class GetGeolocationViaService {
    #region models
    public class Request {
      public string ip { get; set; }
      public int maxmindAccountId { get; set; }
      public string maxmindAccountKey { get; set; }
    }

    public class Response : models.commands.ResponseStatusModel {
      public CityResponse cr { get; set; }
    }
    #endregion

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Response response,
      Request request,
      CancellationToken cancellationToken
    ) {
      response = new Response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        #region validation
        if (string.IsNullOrEmpty(request.ip)) {
          status = $"ip not found";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        var localhosts = new List<string> {
          "127.0.0.1",
          "localhost",
          "::1",
        };
        if (localhosts.Contains(request.ip)) {
          status = $"ip is localhost";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        #endregion

        using (var client = new WebServiceClient(request.maxmindAccountId, request.maxmindAccountKey)) {
          response.cr = client.City(request.ip);

          LogIt.I(JsonConvert.SerializeObject(new {
            //response.cr,

            //most common
            countryIsoCode = response.cr.Country.IsoCode, // 'US'
            countryName = response.cr.Country.Name,  // 'United States'
                                                     //response.cr.Country.Names["zh-CN"]); // '美国'

            mostSpecificSubdivisionName = response.cr.MostSpecificSubdivision.Name, // 'Minnesota'
            MostSpecificSubdivisionIsoCode = response.cr.MostSpecificSubdivision.IsoCode, // 'MN'

            cityName = response.cr.City.Name, // 'Minneapolis'

            postalCode = response.cr.Postal.Code, // '55455'

            lat = response.cr.Location.Latitude,  // 44.9733
            lng = response.cr.Location.Longitude, // -93.2323
          }, Formatting.Indented), cancellationToken);
        }

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        status = $"unexpected error";
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex, cancellationToken);
      } finally {
        request.maxmindAccountKey = "DO_LOG_LOG";
        request.maxmindAccountId = -1; //"DO_LOG_LOG";

        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          //response,
          request,
        }, Formatting.Indented), cancellationToken);
      }
    }
  }
}
