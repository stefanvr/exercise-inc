namespace ExerciseInc.Core.Timers;

/// <summary>
/// The values that make a timer: doc/domain.md › Settings. Durations are whole
/// seconds. An instance is always valid; a timer is its settings, so two equal
/// instances are the same timer.
/// </summary>
public sealed record TimerSettings
{
    /// <summary>59:59, the longest phase the m:ss countdown shows.</summary>
    public const int MaxDuration = 3599;

    public static ValueRange StartDelayRange { get; } = new(5, MaxDuration);
    public static ValueRange WorkRange { get; } = new(1, MaxDuration);
    public static ValueRange RestRange { get; } = new(1, MaxDuration);
    public static ValueRange RepeatsRange { get; } = new(1, 99);

    public TimerSettings(int? startDelay, int work, int rest, int repeats)
    {
        if (startDelay is { } delay)
            Require(StartDelayRange, delay, nameof(startDelay));
        Require(WorkRange, work, nameof(work));
        Require(RestRange, rest, nameof(rest));
        Require(RepeatsRange, repeats, nameof(repeats));

        StartDelay = startDelay;
        Work = work;
        Rest = rest;
        Repeats = repeats;
    }

    /// <summary>Null when the timer has no start delay.</summary>
    public int? StartDelay { get; }
    public int Work { get; }
    public int Rest { get; }
    public int Repeats { get; }

    /// <summary>doc/domain.md › Name.</summary>
    public string Name =>
        $"{Repeats} × {Work}/{Rest} s" + (StartDelay is { } delay ? $" · {delay} s delay" : "");

    private static void Require(ValueRange range, int value, string name)
    {
        if (!range.Contains(value))
            throw new ArgumentOutOfRangeException(name, value, $"Allowed: {range.Min} to {range.Max}.");
    }
}

public readonly record struct ValueRange(int Min, int Max)
{
    public bool Contains(int value) => value >= Min && value <= Max;
}
