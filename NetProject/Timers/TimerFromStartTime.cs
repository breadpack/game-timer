using System;
using BreadPack.GameTimer.Interfaces;

namespace BreadPack.GameTimer.Timers {
    public class TimerFromStartTime : TimerBase {
        public TimeSpan timeInterval { get; }
        
        public TimerFromStartTime(IDateTimeProvider dateTimeProvider, TimeSpan timeInterval) : base(dateTimeProvider) {
            this.timeInterval = timeInterval;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            return lastTime + timeInterval;
        }

    }
}