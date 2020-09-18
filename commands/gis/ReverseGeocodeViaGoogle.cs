using CoarUtils.commands.logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading;

namespace CoarUtils.commands.gis {

  //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452&key=YOUR_API_KEY
  public static class ReverseGeocodeViaGoogle {
    public class request {
      public decimal lat { get; set; }
      public decimal lng { get; set; }
      public string apiKey { get; set; }
    }
    public class response {
      public string address { get; set; }
    }

    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      out response r,
      request m,
      CancellationToken? ct = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      r = new response { };
      try {
        if (m == null) {
          hsc = HttpStatusCode.BadRequest;
          status = "params were null";
          return;
        }
        var resource = "maps/api/geocode/json?latlng=" + m.lat.ToString() + "," + m.lng.ToString() + "&key=" + m.apiKey;
        var client = new RestClient("https://maps.googleapis.com/");
        var request = new RestRequest(resource, Method.GET);
        request.RequestFormat = DataFormat.Json;
        var response = client.Execute(request);
        if (response.ErrorException != null) {
          hsc = HttpStatusCode.BadRequest;
          status = response.ErrorException.Message;
          return;
        }
        if (response.StatusCode != HttpStatusCode.OK) {
          hsc = HttpStatusCode.BadRequest;
          status = $"status was {response.StatusCode.ToString()}";
          return;
        }
        var content = response.Content;
        dynamic json = JObject.Parse(content);
        //dynamic address_components = json.results[0].address_components;

        //for (int i = 0; i < address_components.Count; i++) {
        //  var ac = address_components[i];
        //  var lot = ac.types;
        //  for (int j = 0; j < ac.types.Count; j++) {
        //    var t = (string)ac.types[j].Value;
        //    if (t.Equals("street_number")) {
        //      af.line1 = ac.short_name.Value;
        //      break;
        //    }
        //  }
        //  for (int j = 0; j < ac.types.Count; j++) {
        //    var t = (string)ac.types[j].Value;
        //    if (t.Equals("route")) {
        //      af.street = ac.short_name.Value;
        //      break;
        //    }
        //  }
        //  for (int j = 0; j < ac.types.Count; j++) {
        //    var t = (string)ac.types[j].Value;
        //    if (t.Equals("sublocality")) {
        //      af.city = ac.short_name.Value;
        //      break;
        //    }
        //  }
        //  for (int j = 0; j < ac.types.Count; j++) {
        //    var t = (string)ac.types[j].Value;
        //    if (t.Equals("administrative_area_level_1")) {
        //      af.statecode = ac.short_name.Value;
        //      af.state = ac.long_name.Value;
        //      break;
        //    }
        //  }
        //  for (int j = 0; j < ac.types.Count; j++) {
        //    var t = (string)ac.types[j].Value;
        //    if (t.Equals("country")) {
        //      af.countrycode = ac.short_name.Value;
        //      break;
        //    }
        //  }
        //  for (int j = 0; j < ac.types.Count; j++) {
        //    var t = (string)ac.types[j].Value;
        //    if (t.Equals("postal_code")) {
        //      af.postal = ac.short_name.Value;
        //      break;
        //    }
        //  }

        //}
        //return true;
        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexecpected error";
        return;
      } finally {
        LogIt.I(JsonConvert.SerializeObject(
          new {
            hsc,
            status,
            m,
            //ipAddress = GetPublicIpAddress.Execute(hc),
            //executedBy = GetExecutingUsername.Execute()
          }, Formatting.Indented));
      }
    }
  }
}