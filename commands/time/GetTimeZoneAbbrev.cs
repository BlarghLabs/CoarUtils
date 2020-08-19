using System;
using System.Linq;

namespace CoarUtils.commands.time {

  /// <summary>
  /// use http://mj1856.github.io/TimeZoneNames/
  /// </summary>
  public static class GetTimeZoneAbbrev {
    public static string Execute(string timeZoneId) {
      var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
      var los = tzi.DisplayName.Split(" ");
      var hackAbrev = string.Join("", los);
      return hackAbrev;
    }
  }
}
