using System;
using BreadPack.GameTimer.Interfaces;

namespace BreadPack.GameTimer.DateTimeProviders {
    public class UtcDateTimeProvider : IDateTimeProvider {
        public DateTime     Now          => DateTime.UtcNow;
        public DateTime     UtcNow       => DateTime.UtcNow;
        public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.Utc;
    }
}