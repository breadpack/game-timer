using System;
using Starter.GameTimer.Interfaces;

namespace Starter.GameTimer.DateTimeProviders {
    public class UtcDateTimeProvider : IDateTimeProvider {
        public DateTime     Now          => DateTime.UtcNow;
        public DateTime     UtcNow       => DateTime.UtcNow;
        public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Utc;
    }
}