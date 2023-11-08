using System;
using Starter.GameTimer.Interfaces;

namespace Starter.GameTimer.DateTimeProviders {
    public class SystemDateTimeProvider : IDateTimeProvider {
        public DateTime     Now            => DateTime.Now;
        public DateTime     UtcNow         => DateTime.UtcNow;
        public TimeZoneInfo TimeZoneInfo   => TimeZoneInfo.Local;
    }
}