using CoarUtils.commands.logging;
using CoarUtils.commands.strings;
using PhoneNumbers;
using System;
using System.Globalization;

namespace CoarUtils.commands.validation {
  public class IsValidPhoneNumber {
    public static bool Execute(
      ref string phoneNumber,
      ref string phoneNumberCountryCode
    ) {
      try {
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(phoneNumberCountryCode)) {
          return false;
        }

        //validate country code  
        try {
          var ri = new RegionInfo(phoneNumberCountryCode.ToLower());
        } catch (ArgumentException ae) {
          if (!IsValidCountryCode.Execute(phoneNumberCountryCode.ToLower())) {
            LogIt.W(ae);
            LogIt.W("unrecognized country code (ISO 3166 expected)");
            return false;
          }
        }

        //why dont we try validating as supplied first and if that succeeds then proceed: validate it first before updating by removing non-numerics
        //validate on phone numbers
        var pnu = PhoneNumberUtil.GetInstance();
        var pn = pnu.Parse(phoneNumber, phoneNumberCountryCode.ToUpper());
        if (pnu.IsValidNumber(pn)) {
          phoneNumber = pnu.Format(pn, PhoneNumberFormat.E164);
        } else {
          //normalize phone number
          phoneNumber = StripNonNumericCharacters.Execute(phoneNumber);
          pn = pnu.Parse(phoneNumber, phoneNumberCountryCode.ToUpper());
          if (!pnu.IsValidNumber(pn)) {
            LogIt.W("unrecognized phone number format");
            return false;
          }
          phoneNumber = pnu.Format(pn, PhoneNumberFormat.E164);
        }

        //we stroe in DB as lower
        phoneNumberCountryCode = phoneNumberCountryCode.ToLower();
        return true;
      } catch (Exception ex) {
        LogIt.E(ex);
        return false;
      }
    }
  }
}
