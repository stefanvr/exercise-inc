using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public class StartedTimerTests
{
    private static readonly DateTimeOffset T0 = new(2026, 10, 6, 12, 0, 0, TimeSpan.Zero);

    private static DateTimeOffset At(double seconds) => T0.AddSeconds(seconds);

    [Fact(DisplayName = "domain › Starting a timer: the start delay, then work and rest per repeat, the last rest included")]
    public void PhaseOrder()
    {
        // 10 s delay, then 2 × (30 s work + 5 s rest): done at 80 s.
        var timer = StartedTimer.Start(new TimerSettings(10, 30, 5, 2), T0);

        Assert.Equal(new TimerMoment(TimerState.Running, Phase.StartDelay, null, 2, TimeSpan.FromSeconds(10)), timer.At(At(0)));
        Assert.Equal(new TimerMoment(TimerState.Running, Phase.StartDelay, null, 2, TimeSpan.FromSeconds(0.5)), timer.At(At(9.5)));
        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Work, 1, 2, TimeSpan.FromSeconds(30)), timer.At(At(10)));
        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Rest, 1, 2, TimeSpan.FromSeconds(5)), timer.At(At(40)));
        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Work, 2, 2, TimeSpan.FromSeconds(30)), timer.At(At(45)));
        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Rest, 2, 2, TimeSpan.FromSeconds(1)), timer.At(At(79)));
        Assert.Equal(new TimerMoment(TimerState.Done, null, null, 2, TimeSpan.Zero), timer.At(At(80)));
        Assert.Equal(TimerState.Done, timer.At(At(3600)).State);
    }

    [Fact(DisplayName = "domain › Starting a timer: without a start delay, work starts at once")]
    public void NoStartDelay()
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0);

        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Work, 1, 1, TimeSpan.FromSeconds(30)), timer.At(At(0)));
        Assert.Equal(TimerState.Done, timer.At(At(35)).State);
    }

    [Fact(DisplayName = "domain › Starting a timer: paused holds the countdown; Resume continues from the same moment")]
    public void PauseAndResume()
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0);

        timer.Pause(At(12));
        Assert.Equal(new TimerMoment(TimerState.Paused, Phase.Work, 1, 1, TimeSpan.FromSeconds(18)), timer.At(At(500)));

        timer.Resume(At(500));
        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Work, 1, 1, TimeSpan.FromSeconds(8)), timer.At(At(510)));
    }

    [Fact(DisplayName = "domain › Starting a timer: Restart starts again from the first phase, running")]
    public void Restart()
    {
        var timer = StartedTimer.Start(new TimerSettings(10, 30, 5, 2), T0);
        timer.Pause(At(50));

        timer.Restart(At(100));

        Assert.Equal(new TimerMoment(TimerState.Running, Phase.StartDelay, null, 2, TimeSpan.FromSeconds(10)), timer.At(At(100)));
    }

    [Fact(DisplayName = "domain › Starting a timer: the countdown follows real elapsed time")]
    public void RealElapsedTime()
    {
        // No tick happens in between, as when the app is in the background.
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 8), T0);

        Assert.Equal(new TimerMoment(TimerState.Running, Phase.Rest, 4, 8, TimeSpan.FromSeconds(2)), timer.At(At(138)));
    }

    [Fact(DisplayName = "domain › Starting a timer: pause, resume and restart only from the states that offer them")]
    public void CommandsFollowState()
    {
        var timer = StartedTimer.Start(new TimerSettings(null, 30, 5, 1), T0);
        Assert.Throws<InvalidOperationException>(() => timer.Resume(At(1)));
        Assert.Throws<InvalidOperationException>(() => timer.Restart(At(1)));

        timer.Pause(At(1));
        Assert.Throws<InvalidOperationException>(() => timer.Pause(At(2)));

        timer.Resume(At(2));
        Assert.Throws<InvalidOperationException>(() => timer.Pause(At(40)));
    }

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
