using System;

namespace Starter.GameTimer.Interfaces {
    public abstract class TimerBase : ITimer {
        public IDateTimeProvider DateTimeProvider { get; }
        private readonly TimeSpan defaultLatency;

        public TimerBase(IDateTimeProvider dateTimeProvider, TimeSpan? latency = null) {
            DateTimeProvider = dateTimeProvider;
            defaultLatency = latency ?? TimeSpan.Zero;
        }

        /// <summary>
        ///   마지막 리셋 시간을 기준으로 다음 리셋 시간을 Utc로반환
        /// </summary>
        /// <param name="lastTime"></param>
        /// <param name="useutcTIme"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public DateTime GetResetUtcTime(DateTime lastTime, bool useutcTIme = false) {
            if (lastTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("lastTime.Kind != DateTimeKind.Utc");
            
            var convertTime    = TimeZoneInfo.ConvertTime(lastTime, DateTimeProvider.TimeZoneInfo);
            var nextTime = _GetNextTime(convertTime);
            return TimeZoneInfo.ConvertTimeToUtc(nextTime, DateTimeProvider.TimeZoneInfo);
        }

        protected abstract DateTime _GetNextTime(DateTime lastTime);

        /// <summary>
        /// 주어진 Utc 시간이 Timer 설정 시간을 지났는지 확인
        /// </summary>
        /// <param name="lastTime"></param>
        /// <param name="latency"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool CheckReset(DateTime lastTime, TimeSpan? latency = null) {
            if (lastTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("lastTime.Kind != DateTimeKind.Utc");

            //2초 정도 딜레이를 계산. 너무 칼 같이 계산하면 ms x만큼 차이가 남
            return DateTimeProvider.UtcNow > GetResetUtcTime(lastTime) - (latency ?? defaultLatency);
        }

        /// <summary>
        /// 주어진 Utc 시간으로 부터 남은 시간 확인
        /// </summary>
        /// <param name="lastTime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TimeSpan GetRemainTime(DateTime lastTime) {
            if (lastTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("lastTime.Kind != DateTimeKind.Utc");

            if (CheckReset(lastTime))
                return TimeSpan.Zero;

            return GetResetUtcTime(lastTime) - DateTimeProvider.UtcNow;
        }
    }
}