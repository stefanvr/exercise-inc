#!/usr/bin/env bash
# Session orientation. Run by the SessionStart hook at start, resume, /clear and
# compaction; its output becomes session context. Safe to run by hand.

cd "${CLAUDE_PROJECT_DIR:-$(dirname "$0")/../..}" || exit 0

echo "# Orientation"
if ! git rev-parse --git-dir >/dev/null 2>&1; then
  echo "Not a git repository."
  exit 0
fi

branch=$(git branch --show-current)
echo "Branch: ${branch:-detached HEAD}"

if git rev-parse --verify -q HEAD >/dev/null; then
  echo
  echo "## Recent commits"
  git log --oneline -8
  if [ -n "$branch" ] && [ "$branch" != main ] && git rev-parse --verify -q main >/dev/null; then
    echo
    echo "## On this branch (main..HEAD)"
    git log --oneline main..HEAD
  fi
else
  echo "No commits yet."
fi

work_branches=$(git branch --list 'work/*' --format='%(refname:short)')
if [ -n "$work_branches" ]; then
  echo
  echo "## Work branches"
  echo "$work_branches"
fi

changes=$(git status --short)
if [ -n "$changes" ]; then
  echo
  echo "## Uncommitted"
  echo "$changes" | head -30
fi

echo
echo "## WORK.md"
if [ -f WORK.md ]; then
  cat WORK.md
  [ "$branch" = main ] && echo "(WORK.md on main: stray; it belongs to a work branch)"
  git ls-files --error-unmatch WORK.md >/dev/null 2>&1 && echo "(WORK.md is committed: it must stay local; git rm --cached WORK.md)"
else
  echo "none"
fi

echo
echo "## Open decisions (PROVISIONAL)"
markers=$(git grep -n -I --untracked PROVISIONAL -- ':!.claude' 2>/dev/null | head -50)
echo "${markers:-none}"

[ -f README.md ] || { echo; echo "README.md is missing."; }
[ -f doc/product.md ] || { echo; echo "doc/product.md is missing: the product has not started. The first work establishes its intent (specify)."; }
exit 0
