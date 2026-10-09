using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public class NewTimerTests
{
    private static readonly TimerSettings[] None = [];

    [Fact(DisplayName = "app › New timer: an empty start delay means no delay")]
    public void EmptyStartDelayIsNoDelay()
    {
        var draft = TimerDraft.Of("", "30", "5", "8", None);

        Assert.Equal(FieldState.Empty, draft.StartDelay);
        Assert.Equal(new TimerSettings(null, 30, 5, 8), draft.Settings);
        Assert.True(draft.CanSave);
    }

    [Fact(DisplayName = "app › New timer: Save is disabled while a value is out of its range")]
    public void OutOfRangeDisablesSave()
    {
        var draft = TimerDraft.Of("3", "30", "5", "8", None);

        Assert.Equal(FieldState.OutOfRange, draft.StartDelay);
        Assert.Null(draft.Settings);
        Assert.False(draft.CanSave);
    }

    [Theory(DisplayName = "app › New timer: only whole numbers in range are valid")]
    [InlineData("0")]
    [InlineData("3600")]
    [InlineData("1.5")]
    [InlineData("-3")]
    [InlineData("abc")]
    public void NotAWholeNumberInRange(string work) =>
        Assert.Equal(FieldState.OutOfRange, TimerDraft.Of("", work, "5", "8", None).Work);

    [Fact(DisplayName = "app › New timer: an empty work, rest or repeats disables Save without showing a range")]
    public void EmptyRequiredField()
    {
        var draft = TimerDraft.Of("", "30", "", "8", None);

        Assert.Equal(FieldState.Empty, draft.Rest);
        Assert.False(draft.CanSave);
    }

    [Fact(DisplayName = "app › New timer: Save is disabled while the settings match an existing timer")]
    public void DuplicateDisablesSave()
    {
        var draft = TimerDraft.Of("10", "30", "5", "8", [new TimerSettings(10, 30, 5, 8)]);

        Assert.True(draft.IsDuplicate);
        Assert.False(draft.CanSave);
        Assert.False(TimerDraft.Of("", "30", "5", "8", [new TimerSettings(10, 30, 5, 8)]).IsDuplicate);
    }
}
