using ExerciseInc.Core.Storage;
using ExerciseInc.Core.Timers;
using SQLite;

namespace ExerciseInc.Core.Tests;

public sealed class DataTests : TempDatabase
{
    [Fact(DisplayName = "domain › Data: timers stay until deleted and survive reopening the database")]
    public void TimersSurviveReopening()
    {
        using (var store = new TimerStore(Path))
        {
            store.Add(new TimerSettings(10, 30, 5, 8));
            var deleted = store.Add(new TimerSettings(null, 60, 30, 3));
            store.Add(new TimerSettings(null, 20, 10, 10));
            store.Delete(deleted.Id);
        }

        using var reopened = new TimerStore(Path);
        Assert.Equal(
            [new TimerSettings(10, 30, 5, 8), new TimerSettings(null, 20, 10, 10)],
            reopened.All().Select(timer => timer.Settings));
    }

    [Fact(DisplayName = "architecture › Rules: the database carries its schema version")]
    public void SchemaVersion()
    {
        new TimerStore(Path).Dispose();

        using var db = new SQLiteConnection(Path);
        Assert.Equal(1, db.ExecuteScalar<int>("PRAGMA user_version"));
    }
}
