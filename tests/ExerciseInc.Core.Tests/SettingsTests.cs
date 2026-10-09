using ExerciseInc.Core.Storage;
using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public sealed class SettingsTests : TempDatabase
{
    [Fact(DisplayName = "domain › Settings: a timer has no start delay or one of at least 5 s")]
    public void StartDelayAbsentOrAtLeastFive()
    {
        Assert.Null(new TimerSettings(null, 30, 5, 8).StartDelay);
        Assert.Equal(5, new TimerSettings(5, 30, 5, 8).StartDelay);
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(4, 30, 5, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(0, 30, 5, 8));
    }

    [Fact(DisplayName = "domain › Settings: work and rest are at least 1 s, repeats at least 1")]
    public void LowerBounds()
    {
        Assert.Equal(1, new TimerSettings(null, 1, 1, 1).Work);
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(null, 0, 5, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(null, 30, 0, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(null, 30, 5, 0));
    }

    [Fact(DisplayName = "domain › Settings: each value has an upper bound, so a phase fits m:ss")]
    public void UpperBounds()
    {
        Assert.Equal(3599, new TimerSettings(3599, 3599, 3599, 99).Work);
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(3600, 30, 5, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(null, 3600, 5, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(null, 30, 3600, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TimerSettings(null, 30, 5, 100));
    }

    [Fact(DisplayName = "domain › Settings: a timer is its settings")]
    public void EqualSettingsAreTheSameTimer()
    {
        Assert.Equal(new TimerSettings(10, 30, 5, 8), new TimerSettings(10, 30, 5, 8));
        Assert.NotEqual(new TimerSettings(10, 30, 5, 8), new TimerSettings(null, 30, 5, 8));
    }

    [Fact(DisplayName = "domain › Settings: the store refuses a second timer with the same settings")]
    public void StoreRefusesDuplicate()
    {
        using var store = new TimerStore(Path);
        store.Add(new TimerSettings(null, 30, 5, 8));

        Assert.Throws<InvalidOperationException>(() => store.Add(new TimerSettings(null, 30, 5, 8)));
    }

    [Theory(DisplayName = "domain › Name: generated from the settings")]
    [InlineData(10, 30, 5, 8, "8 × 30/5 s · 10 s delay")]
    [InlineData(null, 30, 5, 8, "8 × 30/5 s")]
    [InlineData(5, 60, 30, 3, "3 × 60/30 s · 5 s delay")]
    [InlineData(null, 90, 30, 3, "3 × 90/30 s")]
    public void Name(int? startDelay, int work, int rest, int repeats, string expected) =>
        Assert.Equal(expected, new TimerSettings(startDelay, work, rest, repeats).Name);
}
