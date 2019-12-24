using System.Net.Mail;
using System.Text;

namespace CoarUtils.commands.validation {
  public class IsValidEmail {
    public static bool Execute(string email, string displayName = null) {
      try {
        if (string.IsNullOrEmpty(email)){
          return false;
        }
        var ma = new MailAddress(email, displayName, Encoding.UTF8);
        return true;
      } catch /* (Exception ex) */ {
        return false;
      }
    }
  }
}