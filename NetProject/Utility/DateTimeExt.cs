using System;
using Starter.GameTimer.Enums;

namespace Starter.GameTimer.Utility {
    public static class DateTimeExt {
        public static EDayOfTheWeekFlag GetWeek(this DateTime date) {
            return (EDayOfTheWeekFlag)(1 << (int)date.DayOfWeek);
        }
    }
}