using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CoarUtils.commands.logging;
using CoarUtils.enums;

namespace CoarUtils.Utils {
  public static class Email {
    // SMTP settings - initialize from configuration at startup
    public static string SmtpHost { get; set; }
    public static int SmtpPort { get; set; } = 587;
    public static string SmtpUserName { get; set; }
    public static string SmtpPassword { get; set; }
    public static bool SmtpEnableSsl { get; set; } = true;

    public static void SendMessage(
      string fromAddress,
      string fromDisplayName,
      string toAddress,
      string toDisplayName,
      string subject,
      string body,
      string replyToAddress,
      string replyToDisplayName,
      string ccAddress,
      string ccDisplayName,
      EmailEngine ee,
      bool isHtml = true,
      bool sendAsync = false,
      bool throwOnError = true
    ) {
      if (string.IsNullOrWhiteSpace(toAddress) || string.IsNullOrWhiteSpace(fromAddress)) {
        throw new Exception("to or from were empty");
      } else {
        switch (ee) {
          case EmailEngine.DotNet:
            #region
            MailMessage mm = new MailMessage();

            mm.From = new MailAddress(fromAddress, fromDisplayName, System.Text.Encoding.UTF8);
            mm.Subject = subject.Trim();
            mm.SubjectEncoding = System.Text.Encoding.UTF8;
            mm.Body = body;
            mm.IsBodyHtml = isHtml;
            mm.BodyEncoding = System.Text.Encoding.UTF8;

            #region handle comma delimited TO
            if (toAddress.Contains(",")) {
              string[] saTo = toAddress.Split((new char[] { ',', ';' }), StringSplitOptions.RemoveEmptyEntries);
              foreach (string sTempToAddress in saTo) {
                mm.To.Add(new MailAddress(sTempToAddress));
              }
            } else {
              mm.To.Add(new MailAddress(toAddress, toDisplayName, System.Text.Encoding.UTF8));
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(replyToAddress)) {
              mm.ReplyToList.Add(new MailAddress(replyToAddress, replyToDisplayName, System.Text.Encoding.UTF8));
            }

            if (!string.IsNullOrWhiteSpace(ccAddress)) {
              mm.CC.Add(new MailAddress(ccAddress, ccDisplayName, System.Text.Encoding.UTF8));
            }

            SendMessage(mm, sendAsync, throwOnError);
            #endregion
            break;
        }
      }
    }

    public static void SendMessageAsAsyncTask(MailMessage mm) {
      Task.Factory.StartNew(() => {
        try {
          SendMessage(mm);
        } catch (Exception ex) {
          LogIt.E(ex);
        }
      });
    }

    public static void SendMessage(MailMessage mm, bool sendAsync = false, bool throwOnError = true) {
      try {
        if (string.IsNullOrEmpty(SmtpHost)) {
          throw new Exception("SMTP settings not configured. Set Email.SmtpHost/SmtpPort/SmtpUserName/SmtpPassword at startup.");
        }

        using (var sc = new SmtpClient(SmtpHost, SmtpPort)) {
          sc.EnableSsl = SmtpEnableSsl || (mm.From.Address.ToLower().Contains("@gmail.com"));
          if (!string.IsNullOrEmpty(SmtpUserName)) {
            sc.Credentials = new NetworkCredential(SmtpUserName, SmtpPassword);
          }

          if (!sendAsync) {
            sc.Send(mm);
          } else {
            string userState = Guid.NewGuid().ToString();
            sc.SendCompleted += new SendCompletedEventHandler(mm_SendCompletedCallback);
            sc.SendAsync(mm, userState);
          }
        }
      } catch (Exception ex) {
        LogIt.E(ex);
        if (throwOnError) {
          throw;
        }
      }
    }

    private static void mm_SendCompletedCallback(object sender, AsyncCompletedEventArgs e) {
      String token = (string)e.UserState;

      if (e.Cancelled) {
        LogIt.W("[" + token + "] Send canceled.");
      }
      if (e.Error != null) {
        LogIt.E("[" + token + "] " + e.Error.ToString());
      }
    }
  }
}
