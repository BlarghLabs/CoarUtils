using CoarUtils.commands.logging;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.ipgeolocation {

  public static class GetGeolocation {
    #region models
    public class request {
      public string ip { get; set; }
      public int maxmindAccountId { get; set; }
      public string maxmindAccountKey { get; set; }
    }

    public class response {
      public CityResponse cr { get; set; }
    }
    #endregion

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out response r,
      request m,
      CancellationToken? ct = null
    ) {
      r = new response { };
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        #region validation
        if (string.IsNullOrEmpty(m.ip)) {
          status = $"ip not found";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        var localhosts = new List<string> {
          "127.0.0.1",
          "localhost",
          "::1",
        };
        if (localhosts.Contains(m.ip)) {
          status = $"ip is localhost";
          hsc = HttpStatusCode.BadRequest;
          return;
        }
        #endregion

        using (var client = new WebServiceClient(m.maxmindAccountId, m.maxmindAccountKey)) {
          r.cr = client.City(m.ip);

          LogIt.I(JsonConvert.SerializeObject(new {
            //r.cr,

            //most common
            countryIsoCode = r.cr.Country.IsoCode, // 'US'
            countryName = r.cr.Country.Name,  // 'United States'
                                              //r.cr.Country.Names["zh-CN"]); // '美国'

            mostSpecificSubdivisionName = r.cr.MostSpecificSubdivision.Name, // 'Minnesota'
            MostSpecificSubdivisionIsoCode = r.cr.MostSpecificSubdivision.IsoCode, // 'MN'

            cityName = r.cr.City.Name, // 'Minneapolis'

            postalCode = r.cr.Postal.Code, // '55455'

            lat = r.cr.Location.Latitude,  // 44.9733
            lng = r.cr.Location.Longitude, // -93.2323
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
        m.maxmindAccountKey = "DO_LOG_LOG";
        m.maxmindAccountId = -1; //"DO_LOG_LOG";

        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          //r,
          m,
        }, Formatting.Indented));
      }
    }
  }
}
