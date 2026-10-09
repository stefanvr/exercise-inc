# Android

## The emulator's first install after booting can fail
Seen: music-inc, 2026-10-08, Android Emulator, Android 16 image (Medium_Phone_API_36.1)

- **What happens.** Right after the emulator has booted, the first install can
  fail although `adb shell getprop sys.boot_completed` reads 1.
- **Why it misleads.** The boot reads as complete.
- **Safe pattern.** Install again.
- **Verify.** The second install succeeds.
- **Stops applying.** When `sys.boot_completed` means installs succeed.

## The emulator's clock is not the computer's
Seen: music-inc, 2026-10-08, Android Emulator, Android 16 image

- **What happens.** The emulator's clock differs from the computer's.
- **Why it misleads.** Logcat's stamps look comparable with the computer's
  time.
- **Safe pattern.** Measure a time on the emulator (`adb shell date +%T.%N`),
  as logcat stamps it.
- **Verify.** Compare `adb shell date` with `date`.
- **Stops applying.** When the emulator keeps the computer's clock.

## An agent drives and sees an Android device through adb
Seen: music-inc, 2026-10-08, Android SDK platform-tools, Pixel 8 Android 17, Android 16 emulator

- **Pattern.** `adb shell input tap|text|keyevent` drives the device;
  `adb exec-out screencap -p > <file>.png` shows it; `adb logcat` and
  `adb shell dumpsys notification --noredact` tell what happened. The
  emulator comes first; the user's phone is for what the emulator cannot show,
  such as another app on the phone, or a WebView freezing. A locked phone
  cannot be seen.
- **Verify.** A screenshot taken after a tap shows the tap's result.
- **Stops applying.** Never; it is plain adb.

## A long-running emulator short of memory fails checks falsely
Seen: health-inc, 2026-10-04, Android Emulator, AVD Medium_Phone_API_36.1 with 2 GB RAM

- **What happens.** After hours of use, an emulator with 2 GB of RAM runs
  short and swaps: an app takes over 20 s to start and Android reports an ANR,
  `uiautomator dump` fails with "null root node", or Windows' adb server
  drops.
- **Why it misleads.** Each reads as a fault of the app or of the check.
- **Safe pattern.** Cold-boot the emulator (Device Manager › Cold Boot) before
  trusting a check run in that state.
- **Verify.** After the cold boot, the app starts within seconds.
- **Stops applying.** When the emulator keeps memory to spare after hours of
  use.

## A check taps a control by its text, found in uiautomator's dump
Seen: health-inc, 2026-10-04, Android SDK platform-tools, Android 16 emulator, .NET MAUI 10

- **Pattern.** `uiautomator dump` lists the screen's views with their `text`
  and `bounds`. A check taps the centre of the view showing a text and reads
  the screen's texts the same way. With screenshots (› An
  agent drives and sees an Android device through adb), that is a repeatable
  surface check with nothing installed on the device:
  ```sh
  dump_screen() {
    adb shell uiautomator dump /sdcard/window.xml >/dev/null
    adb exec-out cat /sdcard/window.xml
  }
  tap() {  # the view showing exactly this text
    read -r x1 y1 x2 y2 <<<"$(dump_screen | grep -o "<node [^>]* text=\"$1\"[^>]*>" |
      head -1 | grep -o 'bounds="[^"]*"' | grep -o '[0-9]\+' | tr '\n' ' ')"
    adb shell input tap $(((x1 + x2) / 2)) $(((y1 + y2) / 2))
  }
  ```
  A control that shows no text is matched by another attribute, such as
  `content-desc` or `resource-id`.
- **Verify.** A tap by text changes the screen's texts as the app should.
- **Stops applying.** Never; it is plain adb.

## Android's backup is checked on the emulator through its local transport
Seen: health-inc, 2026-10-04, Android Emulator, Android 16 image

- **Pattern.** Auto Backup takes the app's data directory, up to 25 MB, while
  the manifest keeps `android:allowBackup="true"`. A check on the emulator
  proves data comes back after a reinstall:
  1. Read `adb shell bmgr enabled`, `bmgr list transports` (the current one is
     starred) and `dumpsys backup` (Auto-restore), and set them back on exit,
     in a `trap`.
  2. `bmgr enable true`, `bmgr autorestore true`,
     `bmgr transport com.android.localtransport/.LocalTransport`.
  3. Record data, then send the app to the background
     (`adb shell input keyevent KEYCODE_HOME`): Android does not back up a
     force-stopped app.
  4. `adb shell bmgr backupnow nl.svrinc.exerciseinc` prints
     `Package nl.svrinc.exerciseinc with result: Success`.
  5. `adb uninstall nl.svrinc.exerciseinc`, install again, and find the data.
- **Verify.** Data recorded before the uninstall is there after the install.
- **Stops applying.** When Android changes how Auto Backup selects or restores
  data.

## The application ID is fixed

It identifies the app on a phone: changing it installs a different app, which
does not see the old app's data. The value is in the app project.
