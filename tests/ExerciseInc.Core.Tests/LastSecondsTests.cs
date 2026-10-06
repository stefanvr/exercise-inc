using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public class LastSecondsTests
{
    private static readonly DateTimeOffset T0 = new(2026, 10, 6, 12, 0, 0, TimeSpan.Zero);

    private static DateTimeOffset At(double seconds) => T0.AddSeconds(seconds);

    [Theory(DisplayName = "style › Last seconds: the last 5 s of a phase flash, light in the first half of each second")]
    [InlineData(24.9, false)] // 0:06
    [InlineData(25.0, true)]  // 0:05 begins
    [InlineData(25.4, true)]
    [InlineData(25.5, false)]
    [InlineData(25.9, false)]
    [InlineData(26.0, true)]  // 0:04 begins
    [InlineData(29.9, false)] // 0:01, second half
    public void FlashInLastFiveSeconds(double elapsed, bool lit)
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0);

        Assert.Equal(lit, timer.At(At(elapsed)).IsFlashLit);
    }

    [Fact(DisplayName = "style › Last seconds: a phase of 5 s or less flashes throughout")]
    public void ShortPhaseFlashesThroughout()
    {
        // Rest of 5 s, from 30 s to 35 s.
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 2), T0);

        Assert.True(timer.At(At(30)).IsInLastSeconds);
        Assert.True(timer.At(At(30)).IsFlashLit);
        Assert.True(timer.At(At(34.9)).IsInLastSeconds);
    }

    [Fact(DisplayName = "style › Last seconds: a paused timer does not flash")]
    public void PausedDoesNotFlash()
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0);
        timer.Pause(At(27));

        Assert.True(timer.At(At(27)).IsInLastSeconds);
        Assert.False(timer.At(At(27)).IsFlashLit);
    }

    [Fact(DisplayName = "style › Last seconds: a done timer does not flash")]
    public void DoneDoesNotFlash() =>
        Assert.False(StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0).At(At(35)).IsFlashLit);
}
