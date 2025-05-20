using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using CoarUtils.commands.debugging;
using CoarUtils.commands.logging;
using Newtonsoft.Json;

namespace CoarUtils.commands.aws.ses {

  public class SendMail {
    /// <summary>
    /// Cannot use async from MVC, only from svc
    /// </summary>
    public static void Execute(
      out HttpStatusCode hsc,
      out string status,
      MailMessage mm,
      string username,
      string password,
      string host,
      int port,
      CancellationToken cancellationToken,
      int max_retries = 3,
      bool sendAsync = false
    ) {
      hsc = HttpStatusCode.BadRequest;
      status = "";
      try {
        using (var sc = new SmtpClient()) {
          sc.Host = host;
          sc.Port = port;
          sc.EnableSsl = true;
          sc.Credentials = new NetworkCredential(username, password);
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
              LogIt.W("attempt:" + i.ToString() + "|" + GetJsonString.Execute(mm), cancellationToken);
              LogIt.E(ex1, cancellationToken);
            }
          }
        }
      } catch (Exception ex) {
        if (cancellationToken.IsCancellationRequested) {
          hsc = HttpStatusCode.BadRequest;
          status = Constants.ErrorMessages.CANCELLATION_REQUESTED_STATUS;
          return;
        }

        LogIt.E(ex, cancellationToken);
        hsc = HttpStatusCode.InternalServerError;
        status = Constants.ErrorMessages.UNEXPECTED_ERROR_STATUS;
        return;
      } finally {
        try {
          LogIt.I(JsonConvert.SerializeObject(new {
            hsc,
            status,
            from = mm == null ? null : new { mm.From.Address, mm.From.DisplayName },
            to = mm == null ? null : mm.To.Select(x => new { x.Address, x.DisplayName }).ToList(),
            replyTo = mm == null ? null : mm.ReplyToList.Select(x => new { x.Address, x.DisplayName }).ToList(),
            subject = mm == null ? null : mm.Subject,
          }, Formatting.Indented), cancellationToken);
        } catch (Exception ex) {
          LogIt.E(ex, cancellationToken);
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
        LogIt.I(ex, CancellationToken.None);
      }
    }
  }
}
