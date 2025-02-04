using System;

namespace BreadPack.GameTimer.Interfaces {
    public interface IDateTimeProvider {
        DateTime Now            { get; }
        DateTime UtcNow         { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }
}