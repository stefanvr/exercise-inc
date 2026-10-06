using ExerciseInc.Core.Storage;
using ExerciseInc.Core.Timers;
using SQLite;

namespace ExerciseInc.Core.Tests;

public sealed class TimerStoreTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"exercise-inc-{Guid.NewGuid():N}.db3");

    public void Dispose() => File.Delete(_path);

    [Fact(DisplayName = "domain › Data: timers stay until deleted and survive reopening the database")]
    public void TimersSurviveReopening()
    {
        using (var store = new TimerStore(_path))
        {
            store.Add(new TimerSettings(10, 30, 5, 8));
            var deleted = store.Add(new TimerSettings(null, 60, 30, 3));
            store.Add(new TimerSettings(null, 20, 10, 10));
            store.Delete(deleted.Id);
        }

        using var reopened = new TimerStore(_path);
        Assert.Equal(
            [new TimerSettings(10, 30, 5, 8), new TimerSettings(null, 20, 10, 10)],
            reopened.All().Select(timer => timer.Settings));
    }

    [Fact(DisplayName = "app › Timer list: timers in the order they were created, newest last")]
    public void CreationOrder()
    {
        using var store = new TimerStore(_path);
        store.Add(new TimerSettings(null, 60, 30, 3));
        store.Add(new TimerSettings(null, 20, 10, 10));
        store.Add(new TimerSettings(5, 30, 5, 8));

        Assert.Equal(
            ["3 × 60/30 s", "10 × 20/10 s", "8 × 30/5 s · 5 s delay"],
            store.All().Select(timer => timer.Settings.Name));
    }

    [Fact(DisplayName = "domain › Settings: the store refuses a second timer with the same settings")]
    public void RefusesDuplicate()
    {
        using var store = new TimerStore(_path);
        store.Add(new TimerSettings(null, 30, 5, 8));

        Assert.Throws<InvalidOperationException>(() => store.Add(new TimerSettings(null, 30, 5, 8)));
    }

    [Fact(DisplayName = "architecture › Rules: the database carries its schema version")]
    public void SchemaVersion()
    {
        new TimerStore(_path).Dispose();

        using var db = new SQLiteConnection(_path);
        Assert.Equal(1, db.ExecuteScalar<int>("PRAGMA user_version"));
    }
}
