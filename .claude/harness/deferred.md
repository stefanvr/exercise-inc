# Deferred and rejected harness options

Harness changes the user rejected or postponed, in the form of `doc/deferred.md`
(`documents.md`).

| date | option | status | reason |
|---|---|---|---|
| 2026-10-08 | A reading rule for command output: `git diff --stat` first, test, build and log output through `tail` or `grep` | rejected | the user chose unfiltered command output: nothing is hidden, and no failure needs a second command to be seen in full |
| 2026-10-08 | Listing fewer skills and connectors every turn: the Claude Docs connector, the claude.ai skills and the bundled skills that are never called (`disableClaudeAiConnectors`, `syncClaudeAiSkills: false`, `skillOverrides`) | deferred | the user judged the saving, about 7k tokens a turn, not worth the optimization yet |
| 2026-10-08 | Moving the phase-bound sections of `.claude/CLAUDE.md` (Delegation, Documents, Learning, Completion, Changing the harness) into the skills and harness files that use them | rejected | saves about 1.2k tokens a turn, and a rule out of view is applied only once its skill or file is loaded; Delegation applies when specifying too |
