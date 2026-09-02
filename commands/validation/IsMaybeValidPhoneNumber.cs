using System.Net;
using CoarUtils.commands.logging;
using CoarUtils.models.commands;
using Newtonsoft.Json;
using PhoneNumbers;

namespace CoarUtils.commands.validation {
  /// <summary>
  /// TARGET command shape - see CLAUDE.md "Legacy -> Target Migration". Converted 2026-09-02.
  ///
  /// The generic catch used to return ex.Message as the status, which puts an exception message
  /// from a third-party parser in front of a caller. It returns UNEXPECTED_ERROR_STATUS now, like
  /// every other command, and the exception is in the log where it belongs.
  /// </summary>
  public class IsMaybeValidPhoneNumber {
    #region models
    public class Request {
      public string numberE164 { get; set; }
    }

    public class Response : ResponseStatusModel { }
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
        if (string.IsNullOrWhiteSpace(request.numberE164)) {
          return Task.FromResult(response = new Response { status = "number not found" });
        }
        if (request.numberE164[0] != '+') {
          return Task.FromResult(response = new Response { status = "e164 format number should start with +" });
        }

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        var phoneNumber = phoneNumberUtil.Parse(request.numberE164, null);
        if (!phoneNumberUtil.IsValidNumber(phoneNumber)) {
          return Task.FromResult(response = new Response { status = "number is not valid" });
        }

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
        }, Formatting.None), cancellationToken);
      }
    }
  }
}
