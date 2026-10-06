#!/usr/bin/env bash
# Build the app, install it through the Windows adb and open it:
#   scripts/run-android.sh          the Debug build, on the running emulator
#   scripts/run-android.sh phone    the Release build, on the phone attached by USB
# Machine preparation: doc/setup.md.
set -euo pipefail
cd "$(dirname "$0")/.."

project=src/ExerciseInc
framework=net10.0-android

case "${1:-emulator}" in
	emulator) configuration=Debug   device=-e ;;
	phone)    configuration=Release device=-d ;;
	*) echo "usage: $0 [emulator|phone]" >&2; exit 2 ;;
esac

app_id=$(dotnet msbuild "$project" -getProperty:ApplicationId -p:TargetFramework=$framework)
dotnet build "$project" -f $framework -c $configuration

# adb is the Windows adb.exe, which cannot read WSL paths.
apk=$project/bin/$configuration/$framework/$app_id-Signed.apk
adb $device install -r "$(wslpath -w "$apk")"
activity=$(adb $device shell cmd package resolve-activity --brief -c android.intent.category.LAUNCHER "$app_id" | tr -d '\r' | tail -1)
adb $device shell am start -W -n "$activity"
