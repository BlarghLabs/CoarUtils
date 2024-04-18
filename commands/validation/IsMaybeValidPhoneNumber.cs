using CoarUtils.commands.logging;
using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Net;

namespace CoarUtils.commands.validation {
  public class IsMaybeValidPhoneNumber {
    public static void Execute(
      string numberE164,
      out HttpStatusCode hsc,
      out string status,
      CancellationToken? ct = null
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";

      try {
        if (string.IsNullOrWhiteSpace(numberE164)) {
          hsc = HttpStatusCode.BadRequest;
          status = "number not found";
          return;
        }

        if (numberE164[0] != '+') {
          hsc = HttpStatusCode.BadRequest;
          status = "e164 format number should start with +";
          return;
        }

        var pnu = PhoneNumberUtil.GetInstance();
        var pn = pnu.Parse(numberE164, null);
        if (!pnu.IsValidNumber(pn)) {
          hsc = HttpStatusCode.BadRequest;
          status = "number is not valid";
          return;
        }

        hsc = HttpStatusCode.OK;
        return;
      } catch (Exception ex) {
        if (ct.HasValue && ct.Value.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        hsc = HttpStatusCode.InternalServerError;
        //status = Constants.UNEXPECTED_ERROR_STATUS; //maybe pass the ex.message here
        status = ex.Message;
        LogIt.E(ex);
      } finally {
        LogIt.I(JsonConvert.SerializeObject(new {
          hsc,
          status,
          numberE164
        }));
      }
    }
  }
}
