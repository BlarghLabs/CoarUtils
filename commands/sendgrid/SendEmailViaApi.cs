using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;
using System.Net;

namespace CoarUtils.commands.sendgrid {
  public static class SendEmailViaApi {
    #region models
    public class Request {
      public string apiKey { get; set; }

    }

    public class Response : ResponseStatusModel {

    }
    #endregion


    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };

      try {
        #region validation

        #endregion

        //if (request.coordinates == null || request.coordinates.Count == 0) {
        //  response.status = "no coordinates provided";
        //  response.hsc = HttpStatusCode.BadRequest;
        //  return response;
        //}
        //if (request.coordinates.Count == 1) {
        //  response.coordinate = request.coordinates.Single();
        //  response.hsc = HttpStatusCode.OK;
        //  response.status = "only one provided";
        //  return response.;
        //}



        response.httpStatusCode = HttpStatusCode.OK;
        return response;
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          response.httpStatusCode = HttpStatusCode.BadRequest;
          response.status = Constants.CANCELLATION_REQUESTED_STATUS;
          return response;
        }
        LogIt.E(ex);
        response.httpStatusCode = HttpStatusCode.InternalServerError;
        response.status = Constants.UNEXPECTED_ERROR_STATUS;
        return response;
      } finally {
        request.apiKey = "DO_NOT_LOG";
        LogIt.I(JsonConvert.SerializeObject(new {
          response.httpStatusCode,
          response.status,
          request,
        }));
      }
    }
  }
}
