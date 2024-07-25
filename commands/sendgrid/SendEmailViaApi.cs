﻿using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Net;

namespace CoarUtils.commands.sendgrid {
  public static class SendEmailViaApi {
    #region models
    public class Request {
      public string apiKey { get; set; }
      public EmailAddress to { get; set; }
      public EmailAddress from { get; set; }
      public string subject { get; set; }
      public string bodyText { get; set; }
      public string bodyHtml { get; set; }
    }

    public class Response : ResponseStatusModel { }
    #endregion


    public static async Task<Response> Execute(
      Request request,
      CancellationToken cancellationToken
    ) {
      var response = new Response { };

      try {
        #region validation
        if (request == null) {
          return new Response { status = "request is null" };
        }
        if (string.IsNullOrWhiteSpace(request.apiKey)) {
          return new Response { status = "apiKey not found" };
        }
        if (request.from == null) {
          return new Response { status = "from not found" };
        }
        if (request.to == null) {
          return new Response { status = "to not found" };
        }
        if (string.IsNullOrWhiteSpace(request.bodyText) && string.IsNullOrWhiteSpace(request.bodyHtml)) {
          return new Response { status = "body not found" };
        }
        #endregion

        var client = new SendGridClient(request.apiKey);
        var msg = MailHelper.CreateSingleEmail(request.from, request.to, request.subject, request.bodyText, request.bodyHtml);
        var sendGridResponse = await client.SendEmailAsync(msg);

        if (!sendGridResponse.IsSuccessStatusCode) {
          response.status = "unable to send";
          response.httpStatusCode = HttpStatusCode.BadRequest;
          return response;
        }

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
