using ExerciseInc.Core.Storage;
using ExerciseInc.Core.Timers;

namespace ExerciseInc.Core.Tests;

public sealed class TimerListTests : TempDatabase
{
    [Fact(DisplayName = "app › Timer list: timers in the order they were created, newest last")]
    public void CreationOrder()
    {
        using var store = new TimerStore(Path);
        store.Add(new TimerSettings(null, 60, 30, 3));
        store.Add(new TimerSettings(null, 20, 10, 10));
        store.Add(new TimerSettings(5, 30, 5, 8));

        Assert.Equal(
            ["3 × 60/30 s", "10 × 20/10 s", "8 × 30/5 s · 5 s delay"],
            store.All().Select(timer => timer.Settings.Name));
    }
}
