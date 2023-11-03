using System;

namespace GameTimer.Utility {
    public static class DateTimeExt {
        public static EDayOfTheWeekFlag GetWeek(this DateTime date) {
            return (EDayOfTheWeekFlag)(1 << (int)date.DayOfWeek);
        }
    }
}