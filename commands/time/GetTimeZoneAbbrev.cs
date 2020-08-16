using System;

namespace CoarUtils.commands.time {
  public static class GetTimeZoneAbbrev {
    public static string Execute(string timeZoneId) {
      var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
      return tzi.DisplayName;
    }
  }
}
