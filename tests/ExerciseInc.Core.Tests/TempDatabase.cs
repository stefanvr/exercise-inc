namespace ExerciseInc.Core.Tests;

/// <summary>A database file of its own for one test, deleted afterwards.</summary>
public abstract class TempDatabase : IDisposable
{
    protected readonly string Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"exercise-inc-{Guid.NewGuid():N}.db3");

    public void Dispose() => File.Delete(Path);
}
