using System;

namespace Starter.GameTimer.Interfaces {
    public interface IDateTimeProvider {
        DateTime Now            { get; }
        DateTime UtcNow         { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }
}