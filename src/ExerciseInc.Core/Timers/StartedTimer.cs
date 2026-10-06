namespace ExerciseInc.Core.Timers;

public enum TimerState { Running, Paused, Done }

public enum Phase { StartDelay, Work, Rest }

/// <summary>
/// A timer that has been started: doc/domain.md › Starting a timer. It counts
/// real elapsed time, given as wall-clock moments by the caller, so it goes on
/// while the app is in the background or the phone sleeps.
/// </summary>
public sealed class StartedTimer
{
    private TimeSpan _elapsedWhilePaused;
    private DateTimeOffset? _resumedAt;

    private StartedTimer(TimerSettings settings, DateTimeOffset now)
    {
        Settings = settings;
        _resumedAt = now;
    }

    public TimerSettings Settings { get; }

    public static StartedTimer Start(TimerSettings settings, DateTimeOffset now) => new(settings, now);

    public void Pause(DateTimeOffset now)
    {
        Require(TimerState.Running, now);
        _elapsedWhilePaused = Elapsed(now);
        _resumedAt = null;
    }

    public void Resume(DateTimeOffset now)
    {
        Require(TimerState.Paused, now);
        _resumedAt = now;
    }

    /// <summary>Starts again from the first phase, running.</summary>
    public void Restart(DateTimeOffset now)
    {
        Require(TimerState.Paused, now);
        _elapsedWhilePaused = TimeSpan.Zero;
        _resumedAt = now;
    }

    public TimerMoment At(DateTimeOffset now)
    {
        var elapsed = Elapsed(now);
        var repeats = Settings.Repeats;

        var delay = TimeSpan.FromSeconds(Settings.StartDelay ?? 0);
        if (elapsed < delay)
            return new TimerMoment(StateAt(), Phase.StartDelay, null, repeats, delay - elapsed);
        elapsed -= delay;

        var work = TimeSpan.FromSeconds(Settings.Work);
        var cycle = work + TimeSpan.FromSeconds(Settings.Rest);
        var index = elapsed.Ticks / cycle.Ticks;
        if (index >= repeats)
            return new TimerMoment(TimerState.Done, null, null, repeats, TimeSpan.Zero);

        var withinCycle = elapsed - cycle * index;
        var repeat = (int)index + 1;
        return withinCycle < work
            ? new TimerMoment(StateAt(), Phase.Work, repeat, repeats, work - withinCycle)
            : new TimerMoment(StateAt(), Phase.Rest, repeat, repeats, cycle - withinCycle);
    }

    private TimerState StateAt() => _resumedAt is null ? TimerState.Paused : TimerState.Running;

    private TimeSpan Elapsed(DateTimeOffset now)
    {
        if (_resumedAt is not { } resumedAt)
            return _elapsedWhilePaused;
        // A wall clock set back must not make time run backwards.
        var sinceResume = now - resumedAt;
        return _elapsedWhilePaused + (sinceResume > TimeSpan.Zero ? sinceResume : TimeSpan.Zero);
    }

    private void Require(TimerState state, DateTimeOffset now)
    {
        var actual = At(now).State;
        if (actual != state)
            throw new InvalidOperationException($"The timer is {actual}, not {state}.");
    }
}
