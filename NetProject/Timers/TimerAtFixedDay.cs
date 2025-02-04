using System;
using BreadPack.GameTimer.Interfaces;

namespace BreadPack.GameTimer.Timers {
    public class TimerAtFixedDay : TimerBase {
        public int day { get; }
        
        public TimerAtFixedDay(IDateTimeProvider dateTimeProvider, int day)
            : base(dateTimeProvider) {
            this.day = day;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            var isNextMonth = lastTime.Day >= day;
            var isNewYear   = isNextMonth && lastTime.Month >= 12;
            var yearOffset  = isNewYear ? 1 : 0;
            var month       = lastTime.Month;
            month = isNextMonth ? month + 1 : month;
            month = month >= 13 ? 1 : month;

            var isFeb = month == 2;

            var resetdate =
                new DateTime(lastTime.Year + yearOffset
                           , month
                           , isFeb && day > 28 ? 28 : day
                             , 0, 0, 0, lastTime.Kind);
            return resetdate;
        }
    }
}