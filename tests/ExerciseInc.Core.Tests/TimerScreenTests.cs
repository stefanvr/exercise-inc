using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public class TimerScreenTests
{
    private static readonly DateTimeOffset T0 = new(2026, 10, 6, 12, 0, 0, TimeSpan.Zero);

    private static DateTimeOffset At(double seconds) => T0.AddSeconds(seconds);

    [Theory(DisplayName = "app › Timer screen: the countdown is m:ss, rounded up to the whole second")]
    [InlineData(0, "0:30")]
    [InlineData(0.001, "0:30")]
    [InlineData(1, "0:29")]
    [InlineData(29.5, "0:01")]
    public void Countdown(double elapsed, string expected)
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0);

        Assert.Equal(expected, timer.At(At(elapsed)).Countdown);
    }

    [Fact(DisplayName = "app › Timer screen: minutes in the countdown")]
    public void CountdownMinutes()
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 3599, 5, 1), T0);

        Assert.Equal("59:59", timer.At(T0).Countdown);
        Assert.Equal("1:30", timer.At(At(3599 - 90)).Countdown);
    }
}
