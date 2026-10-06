# Setup

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

### Bootstrap

Run in WSL, in this order. All steps were run on 2026-10-04.

1. .NET SDK, no root needed:
   ```sh
   curl -sSL -o /tmp/dotnet-install.sh https://dot.net/v1/dotnet-install.sh
   bash /tmp/dotnet-install.sh --channel 10.0 --install-dir ~/.dotnet
   ```
2. Shell environment: Manual steps › Shell environment.
3. The Android workload (on Linux, the `maui` workload is not available;
   `maui-android` is the MAUI workload there):
   ```sh
   dotnet workload install maui-android
   ```
4. The Linux Android SDK and Microsoft OpenJDK, installed by the .NET Android
   tooling. This accepts the Android SDK licence.
   ```sh
   dotnet build src/ExerciseInc -t:InstallAndroidDependencies -f net10.0-android \
     -p:AndroidSdkDirectory=$HOME/Android/Sdk \
     -p:JavaSdkDirectory=$HOME/.jdk/microsoft-openjdk \
     -p:AcceptAndroidSDKLicenses=True
   ```
   The build finds `~/Android/Sdk` by itself afterwards. It finds the JDK
   through `JAVA_HOME`. Without it, the build takes Ubuntu's OpenJDK 17 and
   warns about Ubuntu's Java 21 on the `PATH`, which is a runtime without `jar`.
   Only Microsoft OpenJDK is supported by .NET for Android.
5. Windows side (already present on this machine): Manual steps › Emulator, and
   Manual steps › adb wrapper.

### Confirm it works

1. `dotnet build` succeeds and `dotnet test` passes.
2. Start the emulator (Manual steps › Emulator).
3. `scripts/run-android.sh` installs the app and opens it on the timer list.

## Runtime

The app is installed only on the development emulator. There is no release
signing and no store listing.

## Manual steps

### Shell environment
- who:      the user
- why:      authority: it changes your personal shell profile
- action:   append to `~/.bashrc`:
  ```sh
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"
  export JAVA_HOME="$HOME/.jdk/microsoft-openjdk"
  ```
  Remove or comment out any other `JAVA_HOME` line in `~/.bashrc`.
- when:     once per machine
- expected: a new shell finds `dotnet`, and the Android build uses Microsoft
  OpenJDK
- verify:   in a new shell, `dotnet --version` prints `10.0.x`, and
  `echo $JAVA_HOME` prints the `~/.jdk/microsoft-openjdk` path

### Emulator
- who:      the user
- why:      device: the emulator is a Windows application that needs Windows
  virtualisation
- action:   create the AVD once in Android Studio › Device Manager. Start it
  from WSL:
  ```sh
  /mnt/c/Users/<windows-user>/AppData/Local/Android/Sdk/emulator/emulator.exe -avd Medium_Phone_API_36.1 &
  ```
  or from Android Studio › Device Manager
- when:     the AVD once per machine; start it once per session
- expected: an emulator window on Windows that finishes booting
- verify:   `adb devices` lists `emulator-5554  device`

### adb wrapper
- who:      the user
- why:      device: the emulator listens on the Windows adb server, which a
  Linux adb in WSL cannot reach
- action:   create `~/bin/adb` and make it executable (`chmod +x ~/bin/adb`):
  ```sh
  #!/bin/bash
  exec /mnt/c/Users/<windows-user>/AppData/Local/Android/Sdk/platform-tools/adb.exe "$@"
  ```
  `~/bin` comes before any Linux `platform-tools` on the `PATH`.
- when:     once per machine
- expected: `adb` in WSL is the Windows adb
- verify:   `adb devices` lists the running emulator
