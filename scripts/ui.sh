# Helpers to drive the app on the emulator or the phone through adb, for surface
# checks.
# Source it, then use the functions:
#
#   . scripts/ui.sh
#   tap 'text="New"'            # tap the first node whose attributes match the regex
#   field 2 30                  # replace the 2nd text field's value with 30
#   shot new-timer              # screenshot to $UI_OUT/new-timer.png
#   texts                       # print the texts on screen
#
# Layout dumps and screenshots go to $UI_OUT (default /tmp/exercise-inc-ui).
# The device is $UI_DEVICE: -e for the emulator (default), -d for the phone on USB.

UI_OUT=${UI_OUT:-/tmp/exercise-inc-ui}
UI_DEVICE=${UI_DEVICE:--e}
mkdir -p "$UI_OUT"

# Dump the current layout to $UI_OUT/ui.xml.
dump() {
	adb $UI_DEVICE shell uiautomator dump /sdcard/ui.xml >/dev/null 2>&1
	adb $UI_DEVICE shell cat /sdcard/ui.xml > "$UI_OUT/ui.xml"
}

# Print the centre of the n-th (default 1st) node whose attributes match a regex.
center() {
	python3 - "$UI_OUT/ui.xml" "$1" "${2:-1}" <<'PY'
import re, sys
xml, pattern, n = open(sys.argv[1]).read(), sys.argv[2], int(sys.argv[3])
hits = [node for node in re.findall(r'<node [^>]*>', xml) if re.search(pattern, node)]
if len(hits) < n:
    sys.exit(f"ui.sh: no node {n} matching {pattern}")
x1, y1, x2, y2 = map(int, re.search(r'bounds="\[(\d+),(\d+)\]\[(\d+),(\d+)\]"', hits[n - 1]).groups())
print((x1 + x2) // 2, (y1 + y2) // 2)
PY
}

# Tap the n-th node matching a regex. A node behind the keyboard is covered by it.
tap() {
	dump
	local point
	point=$(center "$1" "${2:-1}") || return 1
	adb $UI_DEVICE shell input tap $point
}

# Replace the n-th text field's value; an empty value clears it.
field() {
	tap 'class="android.widget.EditText"' "$1" || return 1
	adb $UI_DEVICE shell input keyevent KEYCODE_MOVE_END
	for _ in 1 2 3 4 5 6; do adb $UI_DEVICE shell input keyevent KEYCODE_DEL; done
	[ -n "$2" ] && adb $UI_DEVICE shell input text "$2"
}

# Screenshot to $UI_OUT/<name>.png, after an optional delay in seconds.
shot() {
	sleep "${2:-0}"
	adb $UI_DEVICE exec-out screencap -p > "$UI_OUT/$1.png"
}

# The texts on screen, in layout order.
texts() {
	dump
	grep -o 'text="[^"]*"' "$UI_OUT/ui.xml" | grep -v 'text=""' | sed 's/^text=//'
}
