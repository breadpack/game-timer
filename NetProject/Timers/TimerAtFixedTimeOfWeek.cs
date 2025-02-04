using System;
using BreadPack.GameTimer.Enums;
using BreadPack.GameTimer.Interfaces;
using BreadPack.GameTimer.Utility;

namespace BreadPack.GameTimer.Timers {
    public class TimerAtFixedTimeOfWeek : TimerBase {
        private static TimeSpan oneDay = TimeSpan.FromDays(1);

        public TimeOnly          Time { get; }
        public EDayOfTheWeekFlag dayOfTheWeek { get; }
        
        public TimerAtFixedTimeOfWeek(IDateTimeProvider dateTimeProvider, TimeOnly time, EDayOfTheWeekFlag dayOfTheWeek) : base(dateTimeProvider) {
            this.Time = time;
            this.dayOfTheWeek = dayOfTheWeek;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            var lastDay = lastTime.Date;
            if (lastDay <= new DateTime(1, 1, 10))
                lastDay = new DateTime(1, 1, 10);
            
            var week    = lastDay.GetWeek();
            while (!week.HasFlag(dayOfTheWeek)) {
                lastDay -= oneDay;
                week    =  lastDay.GetWeek();
            }

            var intervalDays  = TimeSpan.FromDays(7);
            var intervalTimes = Time;

            while (lastDay <= lastTime) {
                lastDay += intervalDays;
                week    =  lastDay.GetWeek();
                while (!week.HasFlag(dayOfTheWeek)) {
                    lastDay += oneDay;
                    week    =  lastDay.GetWeek();
                }
            }

            return lastDay + intervalTimes;
        }
    }
}