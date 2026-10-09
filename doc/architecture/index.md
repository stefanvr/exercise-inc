# Architecture

What the code and checks must respect of each technology is in its part:
`maui.md`, `android.md`.

## Stack

| concern | choice | why — what it beat |
|---|---|---|
| app framework | .NET MAUI on .NET 10 (LTS), C# | User decision 2026-10-04. .NET 10 beat .NET 11: stable and supported until November 2028, while .NET 11 is not yet generally available (`doc/deferred.md`). |
| target platform | Android only (`net10.0-android`) | The product runs on Android phones (`doc/product.md`). On Linux, MAUI builds nothing else (`maui.md` › On Linux, a MAUI app targets Android only). |
| build environment | Linux (WSL) | User decision 2026-10-04 (`doc/setup/index.md`). Beat a Windows build: see `doc/deferred.md`. |
| persistence | SQLite through sqlite-net-pcl, a database file in the app's private folder | User decision 2026-10-06. It holds the timers now and the plan's later records (sessions, attempts, training logs) without moving data between stores. Beat EF Core with SQLite (heavier, larger app, slower start for no need yet), and a JSON file or Android preferences (no fit for records, so the timers would move later). |
| test tooling | xUnit v3, run with `dotnet test` | User decision 2026-10-06. Beat NUnit and MSTest: equal capability, xUnit is the common choice and the one the supplied plan's feedback names. |

## Structure

- `ExerciseInc.slnx`: the solution.
- `src/ExerciseInc/`: the app, a MAUI project: pages and platform glue.
  `Platforms/Android/` holds the Android entry points. Pages render what
  `ExerciseInc.Core` computes.
- `src/ExerciseInc.Core/`: a plain .NET library with the domain rules
  (`doc/domain.md`, in `Timers/`) and their storage (`Storage/`). It depends on
  nothing in MAUI or Android; the app depends on it (`maui.md` › Linux tests
  reach the rules only in a plain .NET library).
- `tests/ExerciseInc.Core.Tests/`: unit tests of the domain rules.
- `global.json`: makes `dotnet test` use Microsoft.Testing.Platform, which
  xUnit v3 runs on.
- `scripts/`: developer scripts.

## Rules

- **A started timer counts wall-clock time.** The app passes the current UTC
  time to it. A monotonic clock would be immune to clock changes, but on Android
  it stops while the phone sleeps, and the timer must go on with the screen off
  (`doc/domain.md` › Starting a timer).
- **The timer screen is a pushed page, not a modal**, so it can hide the status
  and navigation bars (`doc/app.md` › Timer screen; `maui.md` › A modal page
  opens in its own Android window, where hiding the system bars does nothing).
- **The database carries a schema version from its first release.** Timers
  survive app updates (`doc/domain.md` › Data); each update that changes the
  schema migrates from the stored version.
- **The phone gets the Release build, signed with the release key.** Android
  installs an update only over an app signed with the same key; otherwise the
  app must be uninstalled, which deletes its timers. The debug key is created
  per machine; the release key is created once and backed up
  (`doc/setup/android.md` › Make the app's release key). The project signs
  every Release build with it, so no Release build comes out signed with
  another key (`maui.md` › A sideloaded Release build is an APK signed through
  MSBuild's signing properties).

## Verification

| layer | proves | command |
|---|---|---|
| build | the app compiles for Android | `README.md` › Running it: build |
| unit | the domain rules hold | `README.md` › Running it: test |
| trace | every test names an existing section of `doc/`; which sections of domain, app and style no test names | `README.md` › Running it: trace |
| surface | the app installs, launches and behaves as `doc/app.md` describes | `README.md` › Running it: run, then screenshots |
| phone | the Release build installs over the app on the phone and opens | `README.md` › Running it: run on the phone |
