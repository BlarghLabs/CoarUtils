using System.Net.Mail;
using System.Text;

namespace CoarUtils.commands.email {
  public class IsValid {
    public static bool Execute(string email, string displayName = null) {
      //var b1 = IsEmailValid1(email);
      var b2 = !string.IsNullOrEmpty(email) && IsEmailValidIncludingDisplayName(email, displayName);

      //was: return b1 && b2;
      return b2; //needed xxxx-@hotmail.com to work
    }


    /*
    public static bool IsEmailValid1(string email) {
      bool result = false;
      //string patternLenient = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
      //string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
      //      + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
      //      + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
      //      + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
      //      + @"[a-zA-Z]{2,}))$";
      string patternFromOnline = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
      //result = (new RegexUtilities()).IsValidEmail(value);
      result = (new Regex(patternFromOnline)).IsMatch(email);
      return response;
    }
     * */

    public static bool IsEmailValidIncludingDisplayName(string address, string displayName = null) {
      try {
        MailAddress ma = new MailAddress(address, displayName, Encoding.UTF8);
        return true;
      } catch /* (Exception ex) */ {
        return false;
      }
    }
  }
}