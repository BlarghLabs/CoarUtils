using CoarUtils.commands.logging;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.ipgeolocation {

  public static class GetGeolocation {
    #region models
    public class Request {
      public string ip { get; set; }
      public int maxmindAccountId { get; set; }
      public string maxmindAccountKey { get; set; }
    }

    public class Response {
      public CityResponse cr { get; set; }
    }
    #endregion

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out Response response,
      Request request,
      CancellationToken? ct = null
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
          }, Formatting.Indented));
        }

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        status = $"unexpected error";
        hsc = HttpStatusCode.InternalServerError;
        LogIt.E(ex);

        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = "task cancelled";
          return;
        }
      } finally {
        request.maxmindAccountKey = "DO_LOG_LOG";
        request.maxmindAccountId = -1; //"DO_LOG_LOG";

        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          //response,
          request,
        }, Formatting.Indented));
      }
    }
  }
}
