using System.Net.Mail;
using System.Text;

namespace CoarUtils.commands.validation {
  public class IsValidEmail {
    public static bool Execute(string email, string displayName = null) {
      try {
        if (string.IsNullOrEmpty(email)){
          return false;
        }
        //this allws: x+y@z.io.
        var ma = new MailAddress(email, displayName, Encoding.UTF8);
        //was true, but above will actually parse foo bar <foo@bar.baz> as valid;
        return (ma.Address == email); 
      } catch /* (Exception ex) */ {
        return false;
      }
    }
  }
}