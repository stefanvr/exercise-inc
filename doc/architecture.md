# Architecture

## Stack

| concern | choice | why — what it beat |
|---|---|---|
| app framework | .NET MAUI on .NET 10 (LTS), C# | User decision 2026-10-04. .NET 10 beat .NET 11: stable and supported until November 2028, while .NET 11 is not yet generally available (`doc/deferred.md`). |
| target platform | Android only (`net10.0-android`) | The product runs on Android phones (`doc/product.md`). |
| build environment | Linux (WSL) | User decision 2026-10-04 (`doc/setup.md`). Beat a Windows build: see `doc/deferred.md`. |

## Structure

- `ExerciseInc.slnx`: the solution.
- `src/ExerciseInc/`: the app, a single MAUI project. `Platforms/Android/` holds
  the Android entry points.
- `scripts/`: developer scripts.

## Rules

- **Android is the only target framework.** The other MAUI targets (iOS, Mac
  Catalyst, Windows) cannot be built on Linux, and the product does not need
  them.
- **Debug APKs embed their assemblies.** .NET's fast deployment pushes
  assemblies to the device separately, through the build's own adb. Here the
  APK is installed through the Windows adb (`doc/setup.md`), so it has to be
  self-contained.
- **The application ID is fixed.** It identifies the app on a phone: changing
  it installs a different app, which does not see the old app's data. The value
  is in the app project.

## Verification

| layer | proves | command |
|---|---|---|
| build | the app compiles for Android | `README.md` › Running it: build |
| surface | the app installs, launches and shows its start page | `README.md` › Running it: run, then a screenshot |

There are no automated tests yet because the app holds no logic. Test tooling is
a stack choice, made with the first logic.
