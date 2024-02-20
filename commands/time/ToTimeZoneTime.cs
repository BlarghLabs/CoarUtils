namespace CoarUtils.commands.time {
  //https://weblog.west-wind.com/posts/2015/feb/10/back-to-basics-utc-and-timezones-in-net-web-apps
  public static class ToTimeZoneTime {
    public static DateTime Execute(this DateTime dateTime, string timeZoneId = "UTC") {
      var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
      var offset = (int)timeZoneInfo.BaseUtcOffset.TotalMinutes;
      return dateTime.AddMinutes(offset);
    }

    //this has an issue w/ DST
    /// <summary>
    /// Returns TimeZone adjusted time for a given from a Utc or local time.
    /// Date is first converted to UTC then adjusted.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="timeZoneId"></param>
    /// <returns></returns>
    //public static DateTime Execute(this DateTime time, string timeZoneId = "UTC") {
    //  TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    //  return time.Execute(tzi);
    //}

    /// <summary>
    /// Returns TimeZone adjusted time for a given from a Utc or local time.
    /// Date is first converted to UTC then adjusted.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="timeZoneId"></param>
    /// <returns></returns>
    //public static DateTime Execute(this DateTime time, TimeZoneInfo tzi) {
    //  return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
    //}
  }
}
