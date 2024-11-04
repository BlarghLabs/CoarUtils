//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Net;
//using System.Web;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.AspNetCore.Http;
//using CoarUtils.commands.logging; using CoarUtils.models.commands; using CoarUtils.models;

//namespace CoarUtils.commands.web {
//  public class EchoHeaders {
//    public static void Execute(
//      out HttpStatusCode hsc,
//      out JObject jo
//    ) {
//      jo = new JObject();
//      hsc = HttpStatusCode.BadRequest;
//      try {
//        foreach (string key in HttpContext.Current.Request.Headers.AllKeys) {
//          jo.Add(key, HttpContext.Current.Request.Headers[key]);
//        }
//        hsc = HttpStatusCode.OK;
//        return;
//      } catch (Exception ex) {
//        LogIt.E(ex);
//        hsc = HttpStatusCode.InternalServerError;
//        return;
//      } finally {
//        LogIt.I(JsonConvert.SerializeObject(
//          new {
//            hsc,
//            jo
//          }, Formatting.Indented));
//      }

//    }
//  }
//}
