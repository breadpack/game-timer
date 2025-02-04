using System;
using BreadPack.GameTimer.Interfaces;

namespace BreadPack.GameTimer.Timers {
    public class TimerAtFixedTimeInterval : TimerBase {
        public TimeSpan timeInterval { get; }

        public TimerAtFixedTimeInterval(IDateTimeProvider dateTimeProvider, TimeSpan timeInterval) : base(dateTimeProvider) {
            if (timeInterval >= TimeSpan.FromDays(1))
                throw new ArgumentException("timeInterval must be less than 1 day");

            this.timeInterval = timeInterval;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            var yesterday     = DateTimeProvider.Now.Date.AddDays(-1);
            var resetTime = yesterday + timeInterval;
            while (resetTime <= lastTime) {
                if (resetTime + timeInterval > yesterday.AddDays(1)) {
                    resetTime = yesterday.AddDays(1);
                    yesterday = resetTime;
                }

                resetTime += timeInterval;
            }

            return resetTime;
        }
    }
}