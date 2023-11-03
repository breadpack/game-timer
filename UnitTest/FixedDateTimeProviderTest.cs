using GameTimer;
using GameTimer.DateTimeProviders;

namespace UnitTest;

public class FixedDateTimeProviderTest {
    private IDateTimeProvider _dateTimeProvider;

    [SetUp]
    public void Setup() {
        // We will use a fixed date time provider for testing
        // 테스트하기 편하게 한국시간대로 설정
        _dateTimeProvider = new FixedDateTimeProvider(new DateTime(2023, 4, 10),
                                                      TimeZoneInfo.CreateCustomTimeZone("System", TimeSpan.FromHours(9), "System", "System")); // Fixed date for consistent testing
    }

    [Test]
    public void TimerAtFixedDay_ShouldResetAtSpecifiedDay() {
        // Arrange
        //한국 시간 4월 14일 0시
        var lastTime = new DateTime(2023, 4, 13, 15, 0, 0, DateTimeKind.Utc); // One day before reset
        // 한국시간 4월 15일 0시는 UTC 4월 14일 15시
        var expected = new DateTime(2023, 4, 14, 15, 0, 0, DateTimeKind.Utc);

        // Act
        ITimer timer     = new TimerAtFixedDay(_dateTimeProvider, 15); // Resets on the 15th of every month
        var    resetTime = timer.GetResetUtcTime(lastTime.ToUniversalTime());

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected));
    }

    [Test]
    public void TimerAtFixedTimeInterval_ShouldResetAfterInterval() {
        // Arrange
        var    interval = TimeSpan.FromDays(1); // Resets every day
        var    lastTime = _dateTimeProvider.UtcNow.AddHours(-1); // Last reset was an hour ago
        var    expected = _dateTimeProvider.Now.Date.AddDays(1).ToUniversalTime();

        // Act
        ITimer timer     = new TimerAtFixedTimeInterval(_dateTimeProvider, interval);
        var    resetTime = timer.GetResetUtcTime(lastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected));
    }

    [Test]
    public void TimerAtFixedTime_ShouldResetNextDayIfPastInterval() {
        // Arrange
        var interval = new GameTimer.Utility.TimeOnly(3);
        var lastTime = _dateTimeProvider.Now.AddHours(4).ToUniversalTime(); // 2023년 4월 10일 오전 4시
        var expected = _dateTimeProvider.Now.Date.AddDays(1).AddHours(3).ToUniversalTime(); // 2023년 4월 11일 오전 3시

        // Act
        ITimer timer     = new TimerAtFixedTime(_dateTimeProvider, interval);
        var    resetTime = timer.GetResetUtcTime(lastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected));
    }

    [Test]
    public void TimerFromStartTime_ShouldAddIntervalToLastTime() {
        // Arrange
        var interval = TimeSpan.FromDays(7); // Resets every 7 days from start time
        
        var lastTime = _dateTimeProvider.Now.AddDays(-10).ToUniversalTime(); // Last reset was 10 days ago
        var expected = lastTime.AddDays(7).ToUniversalTime();

        // Act
        ITimer timer     = new TimerFromStartTime(_dateTimeProvider, interval);
        var    resetTime = timer.GetResetUtcTime(lastTime);

        // Assert
        Assert.That(resetTime, Is.EqualTo(expected));
    }

    [Test]
    public void TimerAtFixedTimeOfWeek_Should()
    {
        var lastTime = _dateTimeProvider.Now.ToUniversalTime(); // 2023년 4월 10일 월요일 오전 0시
        var expected = _dateTimeProvider.Now.Date.AddDays(2).AddHours(3).ToUniversalTime(); // 2023년 4월 10일 화요일 오전 3시 
        
        ITimer timer     = new TimerAtFixedTimeOfWeek(_dateTimeProvider, new GameTimer.Utility.TimeOnly(3), EDayOfTheWeekFlag.Wednesday);
        var    resetTime = timer.GetResetUtcTime(lastTime);
        Assert.Multiple(() =>
        {
            Assert.That(resetTime, Is.EqualTo(expected));
            Assert.That(timer.GetResetUtcTime(resetTime), Is.EqualTo(resetTime.AddDays(7)));
        });
    }
}