using Starter.GameTimer.DateTimeProviders;
using Starter.GameTimer.Enums;
using Starter.GameTimer.Interfaces;
using Starter.GameTimer.Timers;
using TimeOnly = Starter.GameTimer.Utility.TimeOnly;

namespace UnitTest;

public class FixedDateTimeProviderTest {
    private static DateTime CreateRandomDateTime() {
        var year   = Random.Shared.Next(2020, 2100);
        var month  = Random.Shared.Next(1, 13);
        var day    = month switch { 2 => Random.Shared.Next(1, 29), 4 or 6 or 9 or 11 => Random.Shared.Next(1, 31), _ => Random.Shared.Next(1, 32) };
        var hour   = Random.Shared.Next(0, 24);
        var minute = Random.Shared.Next(0, 60);
        var second = Random.Shared.Next(0, 60);
        return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
    }

    private static TimeOnly CreateRandomTime() {
        var hour   = Random.Shared.Next(0, 24);
        var minute = Random.Shared.Next(0, 60);
        var second = Random.Shared.Next(0, 60);
        return new(hour, minute, second);
    }

    private static IEnumerable<TestCaseData> TestCases_RandomDateAndOffset {
        get {
            for (int i = 0; i < 1000; ++i) {
                var year      = Random.Shared.Next(2020, 2100);
                var month     = Random.Shared.Next(1, 13);
                var day       = month switch { 2 => Random.Shared.Next(1, 29), 4 or 6 or 9 or 11 => Random.Shared.Next(1, 31), _ => Random.Shared.Next(1, 32) };
                var hour      = Random.Shared.Next(0, 24);
                var minute    = Random.Shared.Next(0, 60);
                var second    = Random.Shared.Next(0, 60);
                var utcOffset = Random.Shared.Next(-14, 14);
                yield return new(CreateRandomDateTime(), utcOffset);
            }
        }
    }

    public IDateTimeProvider Setup(DateTime dateTime, int utcOffset) {
        // We will use a fixed date time provider for testing
        return new FixedDateTimeProvider(
            dateTime,
            TimeZoneInfo.CreateCustomTimeZone(
                "System",
                TimeSpan.FromHours(utcOffset),
                "System",
                "System"
            )
        ); // Fixed date for consistent testing
    }

    [Test, TestCaseSource(nameof(TestCases_RandomDateAndOffset))]
    public void TimerAtFixedDay_ShouldResetAtSpecifiedDay(DateTime dateTime, int utcOffset) {
        var _dateTimeProvider = Setup(dateTime, utcOffset);
        // Arrange
        //한국 시간 4월 14일 0시
        var lastTime = TimeZoneInfo.ConvertTimeToUtc(_dateTimeProvider.Now, _dateTimeProvider.TimeZoneInfo);

        var nextMonth = _dateTimeProvider.Now.AddMonths(1);
        // 한국시간 4월 15일 0시
        var expected = TimeZoneInfo.ConvertTimeToUtc(
            _dateTimeProvider.Now.Day >= 15
                ? new DateTime(nextMonth.Year, nextMonth.Month, 15, 0, 0, 0, DateTimeKind.Unspecified)
                : new DateTime(_dateTimeProvider.Now.Year, _dateTimeProvider.Now.Month, 15, 0, 0, 0, DateTimeKind.Unspecified)
          , _dateTimeProvider.TimeZoneInfo
        );

        // Act
        ITimer timer     = new TimerAtFixedDay(_dateTimeProvider, 15); // Resets on the 15th of every month
        var    resetTime = timer.GetResetUtcTime(lastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected), $"DateTime: {dateTime} offset: {utcOffset}");
    }

    private static IEnumerable<TestCaseData> TestCase_TimerAtFixedDay {
        get {
            for (int i = 0; i < 100; ++i) {
                yield return new(
                    TimeSpan.FromHours(1),
                    new DateTime(2023, 11, 23, 23, 15, 0, DateTimeKind.Unspecified),
                    new DateTime(2023, 11, 24, 0, 0, 0, DateTimeKind.Unspecified),
                    Random.Shared.Next(-14, 14)
                );
            }

            for (int i = 0; i < 100; ++i) {
                yield return new(
                    TimeSpan.FromHours(9),
                    new DateTime(2023, 2, 28, 23, 15, 0, DateTimeKind.Unspecified),
                    new DateTime(2023, 3, 1, 9, 0, 0, DateTimeKind.Unspecified),
                    Random.Shared.Next(-14, 14)
                );
            }

            for (int i = 0; i < 100; ++i) {
                yield return new(
                    TimeSpan.FromHours(7),
                    new DateTime(2023, 7, 30, 15, 15, 0, DateTimeKind.Unspecified),
                    new DateTime(2023, 7, 30, 21, 0, 0, DateTimeKind.Unspecified),
                    Random.Shared.Next(-14, 14)
                );
            }
        }
    }

    [Test, TestCaseSource(nameof(TestCase_TimerAtFixedDay))]
    public void TimerAtFixedTimeInterval_ShouldResetAfterInterval(TimeSpan interval, DateTime lastTime, DateTime expected, int utcOffset) {
        var _dateTimeProvider = Setup(expected + interval * (Random.Shared.NextDouble() - 0.5), utcOffset);
        // Arrange
        var utclastTime = TimeZoneInfo.ConvertTimeToUtc(lastTime, _dateTimeProvider.TimeZoneInfo); // Last reset was an hour ago
        var utcexpected = TimeZoneInfo.ConvertTimeToUtc(expected, _dateTimeProvider.TimeZoneInfo); // Next reset is an hour from now

        // Act
        ITimer timer     = new TimerAtFixedTimeInterval(_dateTimeProvider, interval);
        var    resetTime = timer.GetResetUtcTime(utclastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(utcexpected),
                    $"""
                     Interval: {interval}
                     DateTime: {_dateTimeProvider.Now}
                     LastTime: {lastTime}
                     Expected: {expected}
                     offset: {utcOffset}

                     """);
    }

    [Test, TestCaseSource(nameof(TestCases_RandomDateAndOffset))]
    public void TimerAtFixedTime_ShouldResetNextDayIfPastInterval(DateTime dateTime, int utcOffset) {
        var _dateTimeProvider = Setup(dateTime, utcOffset);
        // Arrange
        var hour     = Random.Shared.Next(0, 24);
        var interval = new TimeOnly(hour);
        var lastTime = TimeZoneInfo.ConvertTimeToUtc(_dateTimeProvider.Now, _dateTimeProvider.TimeZoneInfo); // 2023년 4월 10일 오전 4시
        var expected = TimeZoneInfo.ConvertTimeToUtc(
            _dateTimeProvider.Now.Date.AddHours(hour) + (_dateTimeProvider.Now.Hour >= hour ? TimeSpan.FromDays(1) : TimeSpan.Zero)
          , _dateTimeProvider.TimeZoneInfo
        ); // 2023년 4월 11일 오전 3시

        // Act
        ITimer timer     = new TimerAtFixedTime(_dateTimeProvider, interval);
        var    resetTime = timer.GetResetUtcTime(lastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected),
                    $"""
                     **Parameters**
                     dateTime: {dateTime}
                     offset: {utcOffset}
                     hour: {hour}

                     Expected: {_dateTimeProvider.Now.Date.AddDays(1).AddHours(3)}
                     Actual: {TimeZoneInfo.ConvertTimeFromUtc(resetTime, _dateTimeProvider.TimeZoneInfo)}

                     """);
    }

    [Test, TestCaseSource(nameof(TestCases_RandomDateAndOffset))]
    public void TimerFromStartTime_ShouldAddIntervalToLastTime(DateTime dateTime, int utcOffset) {
        var _dateTimeProvider = Setup(dateTime, utcOffset);
        // Arrange
        var interval = TimeSpan.FromDays(Random.Shared.NextDouble() * 365); // Resets every 7 days from start time

        var lastTime = TimeZoneInfo.ConvertTimeToUtc(_dateTimeProvider.Now, _dateTimeProvider.TimeZoneInfo); // Last reset was 10 days ago
        var expected = TimeZoneInfo.ConvertTimeToUtc(_dateTimeProvider.Now + interval, _dateTimeProvider.TimeZoneInfo);

        // Act
        ITimer timer     = new TimerFromStartTime(_dateTimeProvider, interval);
        var    resetTime = timer.GetResetUtcTime(lastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected), $"DateTime: {dateTime} offset: {utcOffset}");
    }


    private static IEnumerable<TestCaseData> TestCases_TimerAtFixedTimeOfWeek {
        get {
            TestCaseData CreateTestCase(
                int               year,        int month,        int day, int hour, int minute, int second
              , int               expect_year, int expect_month, int expect_day
              , EDayOfTheWeekFlag dayOfTheWeekFlag
            ) {
                var randomTime = CreateRandomTime();
                return new(
                    new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified),
                    new DateTime(expect_year, expect_month, expect_day, randomTime.Hour, randomTime.Minute, randomTime.Second, DateTimeKind.Unspecified),
                    Random.Shared.Next(-14, 14),
                    randomTime,
                    dayOfTheWeekFlag
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2023, 11, 23, 23, 15, 0
                  , 2023, 11, 27,
                    EDayOfTheWeekFlag.Monday
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2021, 4, 5, 13, 15, 0
                  , 2021, 4, 6,
                    EDayOfTheWeekFlag.Tuesday
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2025, 7, 31, 5, 3, 0
                  , 2025, 8, 6,
                    EDayOfTheWeekFlag.Wednesday
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2028, 12, 31, 1, 15, 0
                  , 2029, 1, 4,
                    EDayOfTheWeekFlag.Thursday
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2028, 2, 28, 22, 15, 0
                  , 2028, 3, 3,
                    EDayOfTheWeekFlag.Friday
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2030, 11, 11, 11, 11, 11
                  , 2030, 11, 16,
                    EDayOfTheWeekFlag.Saturday
                );
            }

            for (int i = 0; i < 10; ++i) {
                yield return CreateTestCase(
                    2030, 12, 25, 23, 15, 0
                  , 2030, 12, 29,
                    EDayOfTheWeekFlag.Sunday
                );
            }

            yield return CreateTestCase(1, 1, 1, 0, 0, 0
                                      , 1, 1, 10, EDayOfTheWeekFlag.Wednesday);
        }
    }

    [Test, TestCaseSource(nameof(TestCases_TimerAtFixedTimeOfWeek))]
    public void TimerAtFixedTimeOfWeek_Should(DateTime dateTime, DateTime expect, int utcOffset, TimeOnly time, EDayOfTheWeekFlag dayOfTheWeekFlag) {
        var _dateTimeProvider = Setup(dateTime, utcOffset);
        var lastTime          = _dateTimeProvider.UtcNow;                                              // 2023년 4월 10일 월요일 오전 0시
        var expected          = TimeZoneInfo.ConvertTimeToUtc(expect, _dateTimeProvider.TimeZoneInfo); // 2023년 4월 10일 화요일 오전 3시 

        ITimer timer     = new TimerAtFixedTimeOfWeek(_dateTimeProvider, time, dayOfTheWeekFlag);
        var    resetTime = timer.GetResetUtcTime(lastTime);
        Assert.Multiple(() => {
            Assert.That(resetTime, Is.EqualTo(expected),
                        $"""
                         DateTime: {dateTime}
                         offset: {utcOffset}

                         """);
            Assert.That(timer.GetResetUtcTime(resetTime), Is.EqualTo(resetTime.AddDays(7)),
                        $"""
                         DateTime: {dateTime}
                         offset: {utcOffset}

                         """);
        });
    }
}