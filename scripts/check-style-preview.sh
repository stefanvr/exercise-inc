#!/usr/bin/env bash
# Check the style preview on the emulator: every colour token of the token
# source appears, and every pattern of doc/style.md has its section, in the
# document's order (.claude/skills/style-preview/SKILL.md).
#   scripts/check-style-preview.sh
# It installs the Debug build first (scripts/run-android.sh).
set -euo pipefail
cd "$(dirname "$0")/.."

scripts/run-android.sh >/dev/null
. scripts/ui.sh

tap 'content-desc="More options"'
sleep 1
tap 'text="Style preview"'
sleep 2

# The texts of the whole page, scrolled through, each once, in page order.
read -r width height < <(adb $UI_DEVICE shell wm size | tr -d '\r' | sed 's/.*: //; s/x/ /')
seen="$UI_OUT/style-preview.txt"
: > "$seen"
previous=""
for _ in $(seq 40); do
	screen=$(texts | sed 's/^"//; s/"$//')
	[ "$screen" = "$previous" ] && break
	previous=$screen
	while IFS= read -r text; do
		grep -qxF -- "$text" "$seen" || printf '%s\n' "$text" >> "$seen"
	done <<< "$screen"
	adb $UI_DEVICE shell input swipe $((width / 2)) $((height * 3 / 4)) $((width / 2)) $((height / 4)) 400
	sleep 1
done

failed=0
missing() { echo "missing: $1"; failed=1; }

# Every token in the token source.
while read -r token; do
	grep -qxF -- "$token" "$seen" || missing "token $token"
done < <(grep -o 'x:Key="[^"]*"' src/ExerciseInc/Resources/Styles/Colors.xaml | sed 's/x:Key="//; s/"$//')

# Every pattern of doc/style.md › Visual, then Audible, in the document's order.
patterns=$(awk '/^## /{visual = ($0 == "## Visual")} /^## Audible$/{print "Audible"} visual && /^### /{sub(/^### /, ""); print}' doc/style.md)
last=0
while IFS= read -r pattern; do
	at=$(grep -nxF -- "$pattern" "$seen" | head -1 | cut -d: -f1 || true)
	if [ -z "$at" ]; then
		missing "pattern $pattern"
	elif [ "$at" -lt "$last" ]; then
		echo "out of order: pattern $pattern"
		failed=1
	else
		last=$at
	fi
done <<< "$patterns"

if [ $failed -eq 0 ]; then
	echo "style preview: $(echo "$patterns" | wc -l) patterns in order, every token of the token source shown"
fi
exit $failed
