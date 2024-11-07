using System.Globalization;

//https://stackoverflow.com/questions/19901666/get-date-of-first-and-last-day-of-week-knowing-week-number

namespace CoarUtils.commands.dates {
  public class GetFirstDateOfWeek {
    //I was getting wrong answer for 2/25/22, was returning 2/27/22 not 2/20/22 as expected
    //  public static DateTime Execute(
    //    int year, 
    //    int weekOfYear, 
    //    CultureInfo ci
    //  ) {
    //    DateTime jan1 = new DateTime(year, 1, 1);
    //    int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
    //    DateTime firstWeekDay = jan1.AddDays(daysOffset);
    //    int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
    //    if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3) {
    //      weekOfYear -= 1;
    //    }
    //    var result = firstWeekDay.AddDays(weekOfYear * 7);
    //    return response;
    //  }

    public static DateTime Execute(DateTime date) {
      DayOfWeek fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
      int offset = fdow - date.DayOfWeek;
      DateTime fdowDate = date.AddDays(offset);
      return fdowDate;
    }
  }
}
