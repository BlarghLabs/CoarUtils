namespace CoarUtils.commands.time {
  public static class GetDisplayTime {
    public static string Execute(int? seconds) {
      if (!seconds.HasValue) {
        return "";
      }
      var s =
        ((TimeSpan.FromSeconds((int)seconds.Value).Days == 0) ? "" : (TimeSpan.FromSeconds((int)seconds.Value).Days + "d "))
        + ((TimeSpan.FromSeconds((int)seconds.Value).Hours == 0) ? "" : (TimeSpan.FromSeconds((int)seconds.Value).Hours + "h "))
        + TimeSpan.FromSeconds((int)seconds.Value).Minutes + "m "
        + TimeSpan.FromSeconds((int)seconds.Value).Seconds + "s";
      return s;
    }
  }
}
