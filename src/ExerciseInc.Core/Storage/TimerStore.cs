using ExerciseInc.Core.Timers;
using SQLite;

namespace ExerciseInc.Core.Storage;

/// <summary>A timer as kept on the phone. The id orders timers by creation.</summary>
public sealed record SavedTimer(int Id, TimerSettings Settings);

/// <summary>
/// The timers, kept in the app's SQLite database: doc/domain.md › Data.
/// </summary>
public sealed class TimerStore : IDisposable
{
    /// <summary>doc/architecture/index.md › Rules: the database carries a schema version.</summary>
    public const int SchemaVersion = 1;

    private readonly SQLiteConnection _db;

    public TimerStore(string databasePath)
    {
        _db = new SQLiteConnection(databasePath);
        Migrate();
    }

    /// <summary>All timers, in the order they were created.</summary>
    public IReadOnlyList<SavedTimer> All() =>
        _db.Table<TimerRow>()
            .OrderBy(row => row.Id)
            .ToList()
            .Select(row => new SavedTimer(row.Id, new TimerSettings(row.StartDelay, row.Work, row.Rest, row.Repeats)))
            .ToList();

    public SavedTimer Add(TimerSettings settings)
    {
        if (All().Any(timer => timer.Settings == settings))
            throw new InvalidOperationException($"A timer {settings.Name} already exists.");

        var row = new TimerRow
        {
            StartDelay = settings.StartDelay,
            Work = settings.Work,
            Rest = settings.Rest,
            Repeats = settings.Repeats,
        };
        _db.Insert(row);
        return new SavedTimer(row.Id, settings);
    }

    public void Delete(int id) => _db.Delete<TimerRow>(id);

    public void Dispose() => _db.Dispose();

    private void Migrate()
    {
        var version = _db.ExecuteScalar<int>("PRAGMA user_version");
        if (version > SchemaVersion)
            throw new InvalidOperationException(
                $"The database has schema version {version}; this app knows up to {SchemaVersion}.");

        _db.RunInTransaction(() =>
        {
            if (version < 1)
                _db.CreateTable<TimerRow>();
            _db.Execute($"PRAGMA user_version = {SchemaVersion}");
        });
    }

    [Table("Timer")]
    private sealed class TimerRow
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int? StartDelay { get; set; }
        public int Work { get; set; }
        public int Rest { get; set; }
        public int Repeats { get; set; }
    }
}
