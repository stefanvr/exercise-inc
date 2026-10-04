#!/usr/bin/env bash
# Build the app, install it on the running emulator through the Windows adb and
# open it. Machine preparation: doc/setup.md.
set -euo pipefail
cd "$(dirname "$0")/.."

project=src/ExerciseInc
framework=net10.0-android

app_id=$(dotnet msbuild "$project" -getProperty:ApplicationId -p:TargetFramework=$framework)
dotnet build "$project" -f $framework

# adb is the Windows adb.exe, which cannot read WSL paths.
apk=$project/bin/Debug/$framework/$app_id-Signed.apk
adb install -r "$(wslpath -w "$apk")"
activity=$(adb shell cmd package resolve-activity --brief -c android.intent.category.LAUNCHER "$app_id" | tr -d '\r' | tail -1)
adb shell am start -W -n "$activity"
