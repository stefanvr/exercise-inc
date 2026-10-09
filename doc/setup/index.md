# Setup

The steps for each technology are in its part: `dotnet.md`, `maui.md`,
`android.md`.

## Development

The app is built in WSL 2 (Ubuntu 24.04). The emulator runs on Windows and is
reached from WSL through the Windows `adb.exe`.

### Prerequisites

Versions are those installed on 2026-10-04.

| what | where | version |
|---|---|---|
| .NET SDK | `~/.dotnet` | 10.0.401 |
| `maui-android` workload | in the SDK | 10.0.401.1 |
| Microsoft OpenJDK | `~/.jdk/microsoft-openjdk` | 17.0.14 |
| Android SDK for Linux | `~/Android/Sdk` | platform `android-36`, build-tools 36.0.0 |
| Android SDK for Windows, emulator and AVD | `%LOCALAPPDATA%\Android\Sdk` (Android Studio) | AVD `Medium_Phone_API_36.1` |
| `adb` on the WSL `PATH`, calling the Windows `adb.exe` | `~/bin/adb` | — |
| the user's phone, USB debugging on | Pixel 8 | Android 17 (API 37) |

### Bootstrap

In this order:

1. `dotnet.md` › Install the .NET SDK in the home directory.
2. `maui.md` › Install MAUI for Android on Linux, with its Android SDK and JDK.
3. Windows side: `android.md` › Create an Android emulator, and › Reach the
   phone from WSL through Windows' adb.

### Confirm it works

1. `dotnet build` succeeds and `dotnet test` passes.
2. Start the emulator (`android.md` › Create an Android emulator).
3. `scripts/run-android.sh` installs the app and opens it on the timer list.

## Runtime

The app runs on the user's Android phone, and on the emulator during
development. There is no store listing.

| target | build | signed with | installed by |
|---|---|---|---|
| phone | Release | the release key | `scripts/run-android.sh phone`, over USB |
| emulator | Debug | this machine's debug key, made by the build | `scripts/run-android.sh` |

Every Release build is signed with the release key (`android.md` › Make the
app's release key); without it the build fails. An update installs only over an
app signed with the same key, and uninstalling the app deletes its timers:
without the key, the next update to the phone costs the timers on it. None of
it is in the repository. On another machine, restore it from the backup
(`android.md` › Back up the app's release key off this machine).

### Confirm it is running

1. The phone is attached (`android.md` › Let the computer install on the
   phone).
2. `scripts/run-android.sh phone` installs the app and opens it on the timer
   list.
3. A second run updates the app in place; the timers on the phone stay.
