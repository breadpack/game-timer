using System;

namespace GameTimer {
    public class TimerAtFixedTimeInterval : TimerBase {

        public TimeSpan timeInterval { get; }
        
        public TimerAtFixedTimeInterval(IDateTimeProvider dateTimeProvider, TimeSpan timeInterval) : base(dateTimeProvider) {
            this.timeInterval = timeInterval;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            var resetTime = DateTimeProvider.Now.Date + timeInterval;
            while (resetTime <= lastTime) {
                resetTime += timeInterval;
            }
            return resetTime;
        }
    }
}