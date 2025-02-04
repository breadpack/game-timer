using System;
using BreadPack.GameTimer.Interfaces;

namespace BreadPack.GameTimer.DateTimeProviders {
    public class FixedDateTimeProvider : IDateTimeProvider {
        private readonly DateTime     fixedTime;
        private readonly TimeZoneInfo timeZoneInfo;

        public FixedDateTimeProvider(DateTime time, TimeZoneInfo timeZoneInfo) {
            this.fixedTime = time;
            this.timeZoneInfo = timeZoneInfo;
        }

        public DateTime     Now          => fixedTime;
        public DateTime     UtcNow       => TimeZoneInfo.ConvertTimeToUtc(fixedTime, TimeZoneInfo);
        public TimeZoneInfo TimeZoneInfo => timeZoneInfo;
    }
}