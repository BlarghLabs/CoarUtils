using System;

namespace CoarUtils.commands.dates {
  public class GetLastDayOfWeek {
    public static DateTime Execute(DateTime date) {
      DateTime ldowDate = GetFirstDateOfWeek.Execute(date).AddDays(6);
      return ldowDate;
    }
  }
}
