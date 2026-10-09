# Setup: Android

## Let the computer install on the phone

- who:      the user
- why:      device
- action:
  1. On the phone, Settings › About phone: tap Build number seven times.
  2. Settings › System › Developer options: turn on USB debugging.
  3. Connect the phone over USB and allow this computer when the phone asks.
- when:     once per phone and computer
- expected: the computer installs apps on the phone and drives it over adb
- verify:   `adb devices` lists the phone as `device`, not `unauthorized`
- status:   run 2026-10-06 on the Pixel 8: `adb devices -l` listed it as
            `device`

## Reach the phone from WSL through Windows' adb

- who:      the user, or the agent with the user's go
- why:      device; USB reaches Windows only, and the script lives outside the
            repository
- action:   with Android's SDK installed on Windows, put a script `~/bin/adb`
            on WSL's `PATH`, ahead of any Linux `platform-tools`, that runs
            `/mnt/c/Users/<windows-user>/AppData/Local/Android/Sdk/platform-tools/adb.exe "$@"`.
            Scripts hand it Windows paths (`wslpath -w`). A variable such as
            `ANDROID_SERIAL` reaches it only when `WSLENV` names it:
            `export WSLENV="$WSLENV:ANDROID_SERIAL"`
- when:     once per machine
- expected: `adb` in WSL reaches the phone and the emulator
- verify:   `adb devices` in WSL lists the phone
- status:   in place on this machine since 2026-10-04

## Create an Android emulator

- who:      the user
- why:      device; made in Android Studio's Device Manager
- action:   in Android Studio's Device Manager, create a virtual phone with
            Google Play on a recent Android. WSL: start Windows' emulator from
            WSL with `/mnt/c/Users/<windows-user>/AppData/Local/Android/Sdk/emulator/emulator.exe
            -avd Medium_Phone_API_36.1 &`
- when:     once per machine; start it for each session
- expected: `adb devices` lists `emulator-5554`. With a phone connected as
            well, `ANDROID_SERIAL` names the device a command acts on
- verify:   the app installs and opens on the emulator. The emulator holds none
            of the user's apps or accounts
- status:   AVD `Medium_Phone_API_36.1`, in place on this machine since
            2026-10-04

## Make the app's release key

- who:      the agent
- why:      secret
- action:
  ```sh
  umask 077; dir=~/.config/exercise-inc; mkdir -p $dir
  openssl rand -base64 32 | tr -d '\n' > $dir/release.storepass
  cp $dir/release.storepass $dir/release.keypass
  keytool -genkeypair -keystore $dir/release.keystore -storetype PKCS12 \
    -alias exerciseinc -keyalg RSA -keysize 4096 -validity 10000 \
    -dname "CN=Exercise Inc" -storepass:file $dir/release.storepass
  ```
  The app project reads the keystore and the two password files from these
  paths (`doc/architecture/maui.md` › A sideloaded Release build is an APK
  signed through MSBuild's signing properties).
- when:     once; keep the three files. An app signed with another key does
            not install over the app: it must be uninstalled first, which
            deletes its timers. Running the step again starts over with a new
            key
- expected: every Release build is signed with the key
- verify:   `~/Android/Sdk/build-tools/36.0.0/apksigner verify --print-certs
            <release apk>` shows the certificate's SHA-256, as `keytool -list
            -v` shows it
- status:   run 2026-10-06 by the agent: certificate SHA-256, not a secret,
            `4B:E3:99:7F:8E:84:49:EF:00:FA:39:E3:9B:10:35:83:A7:12:BE:AE:8C:BF:57:67:4B:12:AE:59:DF:36:51:C4`

## Back up the app's release key off this machine

- who:      the user
- why:      secret; the key must outlive this machine
- action:   copy the folder `~/.config/exercise-inc/` with its three files to
            storage off this machine, such as a password manager or an
            encrypted drive. From Windows, WSL's home is
            `\\wsl.localhost\Ubuntu-24.04\home\<linux-user>`. On a new machine,
            put them back at the same paths, mode 600
- when:     once; the restore once per new machine
- expected: the release key survives the loss of this machine
- verify:   `keytool -list -v -keystore <copy>/release.keystore
            -storepass:file <copy>/release.storepass` prints the SHA-256 in
            › Make the app's release key
- status:   reported done by the user on 2026-10-06; the copy was not checked
            from here
