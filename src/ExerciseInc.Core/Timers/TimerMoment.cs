namespace ExerciseInc.Core.Timers;

/// <summary>
/// What a started timer shows at one moment: doc/app.md › Timer screen and
/// doc/style.md › Visual.
/// </summary>
/// <param name="Phase">Null when done.</param>
/// <param name="Repeat">The current repeat, from 1. Null during the start delay and when done.</param>
/// <param name="Remaining">Time left in the phase.</param>
public sealed record TimerMoment(TimerState State, Phase? Phase, int? Repeat, int Repeats, TimeSpan Remaining)
{
    /// <summary>doc/style.md › Last seconds.</summary>
    public static readonly TimeSpan LastSeconds = TimeSpan.FromSeconds(5);

    private static readonly TimeSpan HalfSecond = TimeSpan.FromSeconds(0.5);

    /// <summary>The remaining whole seconds, rounded up: a phase starts on its full length and ends as 1 runs out.</summary>
    public int RemainingSeconds =>
        (int)((Remaining.Ticks + TimeSpan.TicksPerSecond - 1) / TimeSpan.TicksPerSecond);

    /// <summary>The countdown as m:ss.</summary>
    public string Countdown => $"{RemainingSeconds / 60}:{RemainingSeconds % 60:00}";

    public bool IsInLastSeconds => Phase is not null && Remaining <= LastSeconds;

    /// <summary>
    /// Whether the background shows the light version of the phase colour. In
    /// the last seconds of a running timer it is light for the first half of
    /// every countdown second.
    /// </summary>
    public bool IsFlashLit =>
        State == TimerState.Running
        && IsInLastSeconds
        && TimeSpan.FromSeconds(RemainingSeconds) - Remaining < HalfSecond;
}
