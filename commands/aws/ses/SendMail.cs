using CoarUtils.commands.debugging;
using CoarUtils.commands.logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CoarUtils.commands.aws.ses {

  public class SendMail {
    /// <summary>
    /// Cannot use async from MVC, only from svc
    /// </summary>
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      MailMessage mm,
      int max_retries = 3,
      bool sendAsync = false
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var sc = new SmtpClient()) {
          for (int i = 0; i < max_retries; i++) {
            try {
              sc.SendCompleted += new SendCompletedEventHandler(SendCompleted);
              if (sendAsync) {
                var state = new object { };
                sc.SendAsync(mm, state);
              } else {
                sc.Send(mm);
                //log as sent?
              }
              hsc = HttpStatusCode.OK;
              return;
            } catch (Exception ex1) {
              LogIt.W("attempt:" + i.ToString() + "|" + GetJsonString.Execute(mm));
              LogIt.E(ex1);
            }
          }
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        hsc = HttpStatusCode.InternalServerError;
        status = "unexpected error";
        return;
      } finally {
        try {
          LogIt.I(JsonConvert.SerializeObject(new {
            hsc,
            status,
            from = new { mm.From.Address, mm.From.DisplayName },
            to = mm.To.Select(x => new { x.Address, x.DisplayName }).ToList(),
            replyTo = mm.ReplyToList.Select(x => new { x.Address, x.DisplayName }).ToList(),
            subject = mm.Subject,
          }, Formatting.Indented));
        } catch (Exception ex1) {
          LogIt.E(ex1);
        }
      }
    }

    private static void SendCompleted(object sender, AsyncCompletedEventArgs e) {
      try {
        //var u = (user)e.UserState;
        //if we wanted to log email send status
        //if (e.Cancelled) {
        //cancelled
        //}
        //if (e.Error != null) {
        //e.Error.Message
        //} else {
        //success
        //}
      } catch (Exception ex) {
        LogIt.E(ex);
      }
    }
  }
}
