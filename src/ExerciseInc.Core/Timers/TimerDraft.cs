using System.Globalization;

namespace ExerciseInc.Core.Timers;

public enum FieldState
{
    /// <summary>Nothing entered. Valid only for the start delay, where it means no delay.</summary>
    Empty,
    Valid,
    /// <summary>Not a whole number in the field's range; the field shows its range.</summary>
    OutOfRange,
}

/// <summary>
/// The values typed into New timer (doc/app.md › New timer), judged against
/// doc/domain.md › Settings and the timers that already exist.
/// </summary>
public sealed record TimerDraft(
    FieldState StartDelay,
    FieldState Work,
    FieldState Rest,
    FieldState Repeats,
    TimerSettings? Settings,
    bool IsDuplicate)
{
    public bool CanSave => Settings is not null && !IsDuplicate;

    public static TimerDraft Of(
        string startDelay, string work, string rest, string repeats,
        IEnumerable<TimerSettings> existing)
    {
        var (delayState, delay) = Read(startDelay, TimerSettings.StartDelayRange);
        var (workState, workValue) = Read(work, TimerSettings.WorkRange);
        var (restState, restValue) = Read(rest, TimerSettings.RestRange);
        var (repeatsState, repeatsValue) = Read(repeats, TimerSettings.RepeatsRange);

        var complete = delayState != FieldState.OutOfRange
            && workState == FieldState.Valid
            && restState == FieldState.Valid
            && repeatsState == FieldState.Valid;
        var settings = complete
            ? new TimerSettings(delay, workValue!.Value, restValue!.Value, repeatsValue!.Value)
            : null;

        return new TimerDraft(
            delayState, workState, restState, repeatsState,
            settings,
            settings is not null && existing.Contains(settings));
    }

    private static (FieldState, int?) Read(string text, ValueRange range)
    {
        text = text.Trim();
        if (text.Length == 0)
            return (FieldState.Empty, null);
        if (int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
            && range.Contains(value))
            return (FieldState.Valid, value);
        return (FieldState.OutOfRange, null);
    }
}
