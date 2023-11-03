using System;
using GameTimer.Utility;

namespace GameTimer {
    public class TimerAtFixedTime : TimerBase {
        public TimeOnly Time { get; }
        
        public TimerAtFixedTime(IDateTimeProvider dateTimeProvider, TimeOnly time)
            : base(dateTimeProvider) {
            this.Time = time;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            var today = DateTimeProvider.Now.Date;

            var resetTime = today + Time;
            if (resetTime <= lastTime) {
                resetTime = resetTime.AddDays(1);
            }

            return resetTime;
        }
    }
}