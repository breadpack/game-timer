using System;

namespace GameTimer {
    public interface IDateTimeProvider {
        DateTime Now            { get; }
        DateTime UtcNow         { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }
}