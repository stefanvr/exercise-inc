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
| the user's phone, USB debugging on | Pixel 8 | Android 17 (API 37) |

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

The app runs on the user's Android phone, and on the emulator during
development. There is no store listing.

| target | build | signed with | installed by |
|---|---|---|---|
| phone | Release | the release key | `scripts/run-android.sh phone`, over USB |
| emulator | Debug | this machine's debug key, made by the build | `scripts/run-android.sh` |

### Release key

Every Release build is signed with the release key; without it the build
fails. An update installs only over an app signed with the same key, and
uninstalling the app deletes its timers: without the key, the next update to
the phone costs the timers on it. None of it is in the repository.

| what | where |
|---|---|
| keystore, PKCS12, alias `exerciseinc` | `~/.config/exercise-inc/release.keystore` |
| store password; key password, the same value | `~/.config/exercise-inc/release.storepass`, `release.keypass` (mode 600) |
| certificate SHA-256, not a secret | `4B:E3:99:7F:8E:84:49:EF:00:FA:39:E3:9B:10:35:83:A7:12:BE:AE:8C:BF:57:67:4B:12:AE:59:DF:36:51:C4` |

The key was created on 2026-10-06 with the command below. Running it again
starts over with a new key, and the phone then needs an uninstall first.

```sh
umask 077; dir=~/.config/exercise-inc; mkdir -p $dir
openssl rand -base64 32 | tr -d '\n' > $dir/release.storepass
cp $dir/release.storepass $dir/release.keypass
keytool -genkeypair -keystore $dir/release.keystore -storetype PKCS12 \
  -alias exerciseinc -keyalg RSA -keysize 4096 -validity 10000 \
  -dname "CN=Exercise Inc" -storepass:file $dir/release.storepass
```

On another machine, restore it from the backup (Manual steps › Release key
backup).

### Confirm it is running

1. The phone is attached (Manual steps › USB debugging).
2. `scripts/run-android.sh phone` installs the app and opens it on the timer
   list.
3. A second run updates the app in place; the timers on the phone stay.

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

### USB debugging
- who:      the user
- why:      device: the switch and the authorisation live on the phone
- action:   on the phone, turn on Developer options: Settings › About phone ›
  tap Build number seven times (the path varies by manufacturer). Then
  Developer options › USB debugging on. Plug the phone into the Windows PC with
  a data cable and, on the phone, allow USB debugging for this computer with
  "Always allow".
- when:     Developer options once per phone; plugged in for each install
- expected: the Windows adb reaches the phone
- verify:   `adb devices -l` lists the phone as `device`, not `unauthorized`

Run on 2026-10-06; `adb devices -l` listed the phone as `device`.

### Release key backup
- who:      the user
- why:      secret: the key must outlive this machine, which only the user can
  arrange
- action:   copy the folder `~/.config/exercise-inc/` with its three files to
  storage off this machine, such as a password manager or an encrypted drive.
  From Windows it is `\\wsl.localhost\Ubuntu-24.04\home\<linux-user>\.config\exercise-inc`.
  On a new machine, put it back at the same path, the files with mode 600.
- when:     once; the restore once per new machine
- expected: the release key survives the loss of this machine
- verify:   the copy holds the same certificate:
  `keytool -list -v -keystore <copy>/release.keystore -storepass:file <copy>/release.storepass`
  prints the SHA-256 in Runtime › Release key

Reported done by the user on 2026-10-06; the copy was not checked from here.
