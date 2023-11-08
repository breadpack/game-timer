using Starter.GameTimer.DateTimeProviders;
using Starter.GameTimer.Interfaces;

namespace UnitTest; 

public class TimeZoneDateTimeProviderTest {
    private IDateTimeProvider dateTimeProvider;
    [SetUp]
    public void SetUp() {
        dateTimeProvider = new CustomDateTimeProvider(TimeSpan.FromHours(9));
    }
}