#!/usr/bin/env python3
"""Refuse printing more than LIMIT lines of a file into the session at once.

Run by the PreToolUse hook for Bash and Read. Everything printed stays in
context and is re-sent on every later turn (CLAUDE.md › Reading). Covers `cat`,
`nl` and `sed -n` as the last stage of a pipeline, and Read without a limit.
Output sent to a file or into another command is not printed, so it passes.
"""
import glob
import json
import os
import re
import shlex
import sys

LIMIT = 150
NOT_TEXT = ('.png', '.jpg', '.jpeg', '.gif', '.webp', '.pdf', '.ipynb')
SEPARATORS = {'&&', '||', ';', '&', '(', ')', ';;'}


def count_lines(path):
    try:
        with open(path, 'rb') as f:
            return sum(1 for _ in f)
    except OSError:
        return 0


def deny(what, n):
    reason = (
        f'{what} prints {n} lines, over {LIMIT}. Read by section or symbol '
        "(CLAUDE.md › Reading): find headings with grep -n '^#' or a symbol "
        'with grep -n, then read that range. A whole file only when the work '
        'changes most of it: Read with an explicit limit.'
    )
    json.dump({'hookSpecificOutput': {
        'hookEventName': 'PreToolUse',
        'permissionDecision': 'deny',
        'permissionDecisionReason': reason,
    }}, sys.stdout)
    sys.exit(0)


def without_heredocs(command):
    kept, end = [], None
    for line in command.split('\n'):
        if end is not None:
            if line.strip() == end:
                end = None
            continue
        kept.append(line)
        m = re.search(r"<<-?\s*['\"]?(\w+)['\"]?", line)
        if m:
            end = m.group(1)
    return ' ; '.join(kept)


def stages(command):
    """Yield (pipeline's last stage, earlier stages) as token lists."""
    lexer = shlex.shlex(without_heredocs(command), posix=True, punctuation_chars=True)
    lexer.whitespace_split = True
    pipeline, stage = [], []
    for token in list(lexer) + [';']:
        if token in SEPARATORS:
            if stage:
                pipeline.append(stage)
            if pipeline:
                yield pipeline[-1], pipeline[:-1]
            pipeline, stage = [], []
        elif token in ('|', '|&'):
            pipeline.append(stage)
            stage = []
        else:
            stage.append(token)


def printed_words(stage):
    """The stage's words, or None when its standard output goes to a file."""
    words, skip = [], False
    for i, token in enumerate(stage):
        if skip:
            skip = False
            continue
        if token in ('>', '>>', '&>', '&>>', '>|'):
            if i > 0 and stage[i - 1] == '2':
                words.pop()
                skip = True
                continue
            return None
        if token in ('<', '>&', '<&'):
            if token == '>&' and words and words[-1] == '2':
                words.pop()
            skip = True
            continue
        words.append(token)
    while words and re.match(r'^\w+=', words[0]):
        words.pop(0)
    return words


def files(args, cwd):
    found = []
    for arg in args:
        path = os.path.join(cwd, os.path.expanduser(arg))
        found += sorted(glob.glob(path)) or [path]
    return [p for p in found if os.path.isfile(p)]


def sed_lines(args, cwd):
    script, rest, i = None, [], 0
    while i < len(args):
        if args[i] in ('-e', '--expression'):
            script, i = (script or '') + ';' + args[i + 1], i + 2
            continue
        if args[i].startswith('-'):
            i += 1
            continue
        if script is None:
            script = args[i]
        else:
            rest.append(args[i])
        i += 1
    if not script or re.search(r'[/\\]', script):
        return 0
    total = 0
    for path in files(rest, cwd):
        size = count_lines(path)
        for start, end in re.findall(r'(\d+|\$)\s*(?:,\s*(\d+|\$))?\s*p', script):
            first = size if start == '$' else int(start)
            last = first if not end else size if end == '$' else int(end)
            total += max(0, min(last, size) - first + 1)
    return total


def guard_bash(command, cwd):
    for last, earlier in stages(command):
        words = printed_words(last)
        if not words:
            continue
        if words[0] == 'cd' and not earlier:
            cwd = os.path.join(cwd, os.path.expanduser(words[1])) if len(words) > 1 else cwd
            continue
        if earlier:
            continue
        if words[0] in ('cat', 'nl'):
            args = [w for w in words[1:] if not w.startswith('-')]
            n = sum(count_lines(p) for p in files(args, cwd))
            if n > LIMIT:
                deny(f'`{words[0]} {" ".join(args)}`', n)
        elif words[0] == 'sed' and ('-n' in words or '--quiet' in words):
            n = sed_lines(words[1:], cwd)
            if n > LIMIT:
                deny('This `sed -n`', n)


def guard_read(tool_input):
    path = tool_input.get('file_path', '')
    if tool_input.get('limit') or path.lower().endswith(NOT_TEXT):
        return
    n = count_lines(path) - max(int(tool_input.get('offset') or 1), 1) + 1
    if n > LIMIT:
        deny(f'Reading {os.path.basename(path)} without a limit', n)


def main():
    event = json.load(sys.stdin)
    tool_input = event.get('tool_input') or {}
    if event.get('tool_name') == 'Read':
        guard_read(tool_input)
    elif event.get('tool_name') == 'Bash':
        try:
            guard_bash(tool_input.get('command', ''), event.get('cwd') or os.getcwd())
        except (ValueError, IndexError):
            pass


if __name__ == '__main__':
    main()
