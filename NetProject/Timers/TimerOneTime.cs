using System;
using Starter.GameTimer.Interfaces;

namespace Starter.GameTimer.Timers {
    public class TimerOneTime : TimerBase {
        public TimeSpan timeInterval { get; }
        
        public TimerOneTime(IDateTimeProvider dateTimeProvider, TimeSpan timeInterval) : base(dateTimeProvider) {
            this.timeInterval = timeInterval;
        }

        protected override DateTime _GetNextTime(DateTime lastTime) {
            var beginDate = new DateTime(2000, 1, 1);
            var resetDate = beginDate + timeInterval;

            // 반복이 없으니 정해진 날짜 지나갔으면 영영 사용되지 않음.
            return lastTime > resetDate ? DateTime.MaxValue : resetDate;
        }

    }
}