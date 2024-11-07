namespace CoarUtils.commands.time {
  public static class GetRelativeTime {
    public static string Execute(
      DateTime? dt
    ) {
      if (!dt.HasValue) {
        return "na";
      }

      const int SECOND = 1;
      const int MINUTE = 60 * SECOND;
      const int HOUR = 60 * MINUTE;
      const int DAY = 24 * HOUR;
      const int MONTH = 30 * DAY;

      var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Value.Ticks);
      var delta = Math.Abs(ts.TotalSeconds);

      if (ts.TotalSeconds < -3)
        return "in the future";

      if (ts.TotalSeconds < 0)
        return "just now";

      if (delta < 1 * MINUTE)
        return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " second(s) ago";

      if (delta < 2 * MINUTE)
        return "a minute ago";

      if (delta < 45 * MINUTE)
        return ts.Minutes + " minute(s) ago";

      if (delta < 90 * MINUTE)
        return "an hour ago";

      if (delta < 24 * HOUR)
        return ts.Hours + " hour(s) ago";

      if (delta < 48 * HOUR)
        return "yesterday";

      if (delta < 30 * DAY)
        return ts.Days + " day(s) ago";

      if (delta < 12 * MONTH) {
        var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
        var monthsDouble = (double)ts.Days / (double)30;
        return monthsDouble <= 1
          ? "one month ago"
          : (
            monthsDouble.ToString("n1").EndsWith(".0")
              ? months + " month(s) ago"
              : monthsDouble.ToString("n1") + " month(s) ago"
          )
        ;
      } else {
        var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
        var yearsDouble = (double)ts.Days / (double)365;
        return yearsDouble <= 1
          ? "one year ago"
          : (
            yearsDouble.ToString("n1").EndsWith(".0")
              ? years + " year(s) ago"
              : yearsDouble.ToString("n1") + " year(s) ago"
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

    public static string ExecuteCondensed(
      DateTime? dt
    ) {
      if (!dt.HasValue) {
        return "na";
      }

      const int SECOND = 1;
      const int MINUTE = 60 * SECOND;
      const int HOUR = 60 * MINUTE;
      const int DAY = 24 * HOUR;
      const int MONTH = 30 * DAY;

      var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Value.Ticks);
      var delta = Math.Abs(ts.TotalSeconds);

      if (ts.TotalSeconds < -3)
        return "in the future";

      if (ts.TotalSeconds <= 1)
        return "just now";

      if (delta < 1 * MINUTE)
        return ts.Seconds == 1 ? "1 sec ago" : ts.Seconds + " sec ago";

      if (delta < 2 * MINUTE)
        return "1 min ago";

      if (delta < 45 * MINUTE)
        return ts.Minutes + " min ago";

      if (delta < 90 * MINUTE)
        return "1 hr ago";

      if (delta < 24 * HOUR)
        return ts.Hours + " hr ago";

      if (delta < 48 * HOUR)
        return "yesterday";

      if (delta < 30 * DAY)
        return ts.Days + " days ago";

      if (delta < 12 * MONTH) {
        var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
        var monthsDouble = (double)ts.Days / (double)30;
        return monthsDouble <= 1
          ? "one mo ago"
          : (
            monthsDouble.ToString("n1").EndsWith(".0")
              ? months + " mo ago"
              : monthsDouble.ToString("n1") + " mo ago"
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
  }
}