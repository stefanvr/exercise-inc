# Architecture

## Stack

| concern | choice | why — what it beat |
|---|---|---|
| app framework | .NET MAUI on .NET 10 (LTS), C# | User decision 2026-10-04. .NET 10 beat .NET 11: stable and supported until November 2028, while .NET 11 is not yet generally available (`doc/deferred.md`). |
| target platform | Android only (`net10.0-android`) | The product runs on Android phones (`doc/product.md`). |
| build environment | Linux (WSL) | User decision 2026-10-04 (`doc/setup.md`). Beat a Windows build: see `doc/deferred.md`. |
| persistence | SQLite through sqlite-net-pcl, a database file in the app's private folder | User decision 2026-10-06. It holds the timers now and the plan's later records (sessions, attempts, training logs) without moving data between stores. Beat EF Core with SQLite (heavier, larger app, slower start for no need yet), and a JSON file or Android preferences (no fit for records, so the timers would move later). |
| test tooling | xUnit v3, run with `dotnet test` | User decision 2026-10-06. Beat NUnit and MSTest: equal capability, xUnit is the common choice and the one the supplied plan's feedback names. |

## Structure

- `ExerciseInc.slnx`: the solution.
- `src/ExerciseInc/`: the app, a MAUI project: pages and platform glue.
  `Platforms/Android/` holds the Android entry points. Pages render what
  `ExerciseInc.Core` computes.
- `src/ExerciseInc.Core/`: a plain .NET library with the domain rules
  (`doc/domain.md`, in `Timers/`) and their storage (`Storage/`). It depends on
  nothing in MAUI or Android; the app depends on it.
- `tests/ExerciseInc.Core.Tests/`: unit tests of the domain rules.
- `scripts/`: developer scripts.

## Rules

- **Android is the only target framework.** The other MAUI targets (iOS, Mac
  Catalyst, Windows) cannot be built on Linux, and the product does not need
  them.
- **Debug APKs embed their assemblies.** .NET's fast deployment pushes
  assemblies to the device separately, through the build's own adb. Here the
  APK is installed through the Windows adb (`doc/setup.md`), so it has to be
  self-contained.
- **Domain rules stay out of the app project.** A MAUI Android project cannot
  be referenced by tests that run on Linux, so the rules live in
  `ExerciseInc.Core`, which targets plain .NET.
- **A started timer counts wall-clock time.** The app passes the current UTC
  time to it. A monotonic clock would be immune to clock changes, but on Android
  it stops while the phone sleeps, and the timer must go on with the screen off
  (`doc/domain.md` › Starting a timer).
- **The database carries a schema version from its first release.** Timers
  survive app updates (`doc/domain.md` › Data); each update that changes the
  schema migrates from the stored version.
- **The application ID is fixed.** It identifies the app on a phone: changing
  it installs a different app, which does not see the old app's data. The value
  is in the app project.

## Verification

| layer | proves | command |
|---|---|---|
| build | the app compiles for Android | `README.md` › Running it: build |
| unit | the domain rules hold | `README.md` › Running it: test |
| surface | the app installs, launches and behaves as `doc/app.md` describes | `README.md` › Running it: run, then screenshots |
