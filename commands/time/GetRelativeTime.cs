using System;

namespace CoarUtils.commands.time {
  public static class GetRelativeTime {
    public static string Execute(DateTime dt) {
      const int SECOND = 1;
      const int MINUTE = 60 * SECOND;
      const int HOUR = 60 * MINUTE;
      const int DAY = 24 * HOUR;
      const int MONTH = 30 * DAY;

      var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
      var delta = Math.Abs(ts.TotalSeconds);

      if (ts.TotalSeconds < 0)
        return " just now";

      if (delta < 1 * MINUTE)
        return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

      if (delta < 2 * MINUTE)
        return "a minute ago";

      if (delta < 45 * MINUTE)
        return ts.Minutes + " minutes ago";

      if (delta < 90 * MINUTE)
        return "an hour ago";

      if (delta < 24 * HOUR)
        return ts.Hours + " hours ago";

      if (delta < 48 * HOUR)
        return "yesterday";

      if (delta < 30 * DAY)
        return ts.Days + " days ago";

      if (delta < 12 * MONTH) {
        var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
        var monthsDouble = (double)ts.Days / (double)30;
        return monthsDouble <= 1
          ? "one month ago"
          : (
            monthsDouble.ToString("n1").EndsWith(".0")
              ? months + " months ago"
              : monthsDouble.ToString("n1") + " months ago"
          )
        ;
      } else {
        var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
        var yearsDouble = (double)ts.Days / (double)365;
        return yearsDouble <= 1
          ? "one year ago"
          : (
            yearsDouble.ToString("n1").EndsWith(".0")
              ? years + " years ago"
              : yearsDouble.ToString("n1") + " years ago"
          )
        ;
      }
    }

    public static string ExecuteFromNullable(DateTime? dt) {
      if (!dt.HasValue) {
        return "";
      }
      return Execute(dt.Value);
    }
  }
}