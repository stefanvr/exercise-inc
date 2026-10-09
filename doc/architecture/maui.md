# .NET MAUI

## On Linux, a MAUI app targets Android only
Seen: exercise-inc, 2026-10-04, .NET SDK 10.0.401, maui-android 10.0.401.1

- **Pattern.** The app project names one target framework,
  `<TargetFramework>net10.0-android</TargetFramework>`, and keeps only
  `Platforms/Android/`. The stock template's iOS, Mac Catalyst and Windows
  targets cannot be built on Linux.
- **Verify.** `dotnet build` succeeds on Linux.
- **Stops applying.** When the app needs another platform, which builds on
  macOS or Windows.

## A Debug APK installed through another adb embeds its assemblies
Seen: exercise-inc, 2026-10-04, .NET SDK 10.0.401, maui-android 10.0.401.1

- **Pattern.** `<EmbedAssembliesIntoApk>true</EmbedAssembliesIntoApk>` in the
  app project. .NET's fast deployment leaves the assemblies out of a Debug APK
  and pushes them to the device through the build's own adb. An APK installed
  through another adb, such as Windows' from WSL (`doc/setup/android.md` ›
  Reach the phone from WSL through Windows' adb), has to carry them.
- **Verify.** A Debug APK installed with `adb install` opens on the emulator.
- **Stops applying.** When the build installs through the adb that reaches the
  device.

## Linux tests reach the rules only in a plain .NET library
Seen: exercise-inc, 2026-10-06, .NET SDK 10.0.401, xUnit v3

- **Pattern.** The rules, and whatever else tests must reach, live in a library
  that targets plain `net10.0` and knows nothing of MAUI; the app project
  depends on it. A test project on Linux cannot reference a MAUI Android
  project. The tests then run with `dotnet test`, without a device.
- **Verify.** `dotnet test` passes on Linux with no device attached.
- **Stops applying.** When a test project on Linux can reference an
  `-android` project.

## A modal page opens in its own Android window, where hiding the system bars does nothing
Seen: exercise-inc, 2026-10-06, .NET SDK 10.0.401, maui-android 10.0.401.1, Android 16 emulator

- **What happens.** MAUI shows a modal page (`PushModalAsync`) in a separate
  Android window. Hiding the status and navigation bars through the activity's
  window (`WindowCompat.GetInsetsController(window, window.DecorView)
  .Hide(WindowInsetsCompat.Type.SystemBars())`) leaves them on screen.
- **Why it misleads.** The same code hides them over a pushed page.
- **Safe pattern.** Show a full-screen page as a pushed page (`PushAsync`).
- **Verify.** The pushed page shows with no status or navigation bar.
- **Stops applying.** When MAUI shows modal pages in the activity's window.

## A sideloaded Release build is an APK signed through MSBuild's signing properties
Seen: exercise-inc, 2026-10-06, .NET SDK 10.0.401, maui-android 10.0.401.1, Pixel 8 Android 17

- **Pattern.** For Release, the app project sets `AndroidPackageFormat` to
  `apk` (a Release build is otherwise an app bundle, for a store, which adb
  does not install), `AndroidKeyStore` to `true`, `AndroidSigningKeyStore` and
  `AndroidSigningKeyAlias` to the key (`doc/setup/android.md` › Make the app's
  release key; no properties file is needed), and `AndroidSigningStorePass` and
  `AndroidSigningKeyPass` to `file:<path>`, one file each: apksigner reads
  successive lines of a file it is given twice. The key and its password files
  stay outside the repository, mode 600.
- **Verify.** A second Release install over the app on the phone updates it in
  place, and its data stays.
- **Stops applying.** When the app is published through a store.
