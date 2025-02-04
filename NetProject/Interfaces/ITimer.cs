using System;

namespace BreadPack.GameTimer.Interfaces {
    public interface ITimer {
        DateTime GetResetUtcTime(DateTime lastTime, bool      useutcTIme = false);
        bool     CheckReset(DateTime      lastTime, TimeSpan? latency    = null);
        TimeSpan GetRemainTime(DateTime   lastTime);
    }
}