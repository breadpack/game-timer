using System;
using Starter.GameTimer.Interfaces;

namespace Starter.GameTimer.DateTimeProviders {
    public class CustomDateTimeProvider : IDateTimeProvider {
        private readonly TimeZoneInfo timeZoneInfo;

        public CustomDateTimeProvider(TimeSpan offsetFromUtc) {
            timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("System", offsetFromUtc, "System", "System");
        }

        public DateTime     Now          => TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
        public DateTime     UtcNow       => DateTime.UtcNow;
        public TimeZoneInfo TimeZoneInfo => timeZoneInfo;
    }
}