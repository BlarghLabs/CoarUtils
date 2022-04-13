using System;
using System.Globalization;

namespace CoarUtils.commands.dates {
  public class GetFirstDateOfWeek {
    public static DateTime Execute(
      int year, 
      int weekOfYear, 
      CultureInfo ci
    ) {
      DateTime jan1 = new DateTime(year, 1, 1);
      int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
      DateTime firstWeekDay = jan1.AddDays(daysOffset);
      int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
      if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3) {
        weekOfYear -= 1;
      }
      return firstWeekDay.AddDays(weekOfYear * 7);
    }
  }
}
