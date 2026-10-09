# Harness

Version 12 — 2026-10-09.

Operating instructions for building a product with Claude Code. They live inside
the product's repository, so they load in every session mode; an `@import` from
outside the project tree does not load under `claude -p`.

## Files

| path | owner | holds |
|---|---|---|
| `.claude/CLAUDE.md` | harness | always loaded: role, communication, where truth lives, the decision rule, the loop, delegation, repository discipline |
| `.claude/harness/documents.md` | harness | what goes in each project document, their dependencies, writing rules, growth, markers, traceability, `WORK.md` |
| `.claude/harness/software-design.md` | harness | implementation-design preferences, read in `build` |
| `.claude/harness/deferred.md` | harness | rejected and postponed harness changes, with date and reason |
| `.claude/skills/*/SKILL.md` | harness | one procedure per phase or side task, loaded when used |
| `.claude/hooks/orient.sh` | harness | session orientation, run by the SessionStart hook |
| `.claude/hooks/read-guard.py` | harness | refuses printing more than 150 lines of a file at once, run by the PreToolUse hook for Bash and Read |
| `.claude/settings.json` | shared | the SessionStart and PreToolUse hooks, the baseline git permissions, `autoDreamEnabled: false` (learnings go to project documents, not to memory) and the deny rules for built-in tools the workflow never calls (a tool denied by its bare name is not sent in any turn's context) belong to the harness; the project adds its own check commands. Harness-owned `allow` entries are the `git` and `.claude/hooks/` ones; every other entry is the project's |
| everything else | project | |

Loop skills: `specify`, `build`, `present`, `land`. Side skills: `tech`,
`setup`, `style-preview`, `sanity`. Each can also be invoked by name, as
`/specify` and so on.

## Skill descriptions

Keep each skill's `description` under about 150 characters and quote it in the
frontmatter. The skill listing a session receives has a character budget
(`SLASH_COMMAND_TOOL_CHAR_BUDGET`); past it, skills are listed by name only, and
a skill whose description is missing is less likely to be chosen. `CLAUDE.md`
names the skill for each phase, so the loop does not depend on descriptions
alone.

## Source and copies

The source of this harness is the factory's `harness/template/.claude/`; each
project keeps a committed copy, so it loads without the factory. The factory's
`harness/bin/harness` keeps the copies in step:

- `harness status`: every project's version, and whether its copy differs from
  the source.
- `harness diff <project>`: the differences, file by file.
- `harness upgrade <project>`: source to project, or an install into a project
  without one. It copies the harness-owned files, merges `.claude/settings.json`,
  adds `WORK.md` to `.gitignore` and prints the upgrade notes below for the
  versions crossed. Apply those in the project, then commit.
- `harness promote <project>`: project to source, once the project has raised
  the version.

Without the factory, copy `.claude/` by hand, without `settings.local.json`, keep
the hooks executable and merge `.claude/settings.json` by the rule above.

## Using it in a new project

`harness upgrade <project>` on a clean repository installs it. Without
`doc/product.md`, the first work establishes the product's intent.

## Changing the harness

Projects never edit harness-owned files except to change the harness: a project
rule goes into the project document that owns its subject. A harness improvement
found in a project is made in the project's copy on a work branch, raises the
version above and adds its upgrade note below. Once it has landed, `harness
promote <project>` carries it to the source; other projects take it with
`harness upgrade` when they are next worked on.

## Upgrade notes

What a version needs beyond what `harness upgrade` does:

- 2 → 3: add `WORK.md` to `.gitignore` (done by `harness upgrade`).
- 3 → 4: give a `WORK.md` in progress the `Phase` and `Landing` lines and the
  For review section.
- 4 → 5: bring what another project settles into the owning documents
  (`documents.md` › Stand alone).
- 5 → 6: reshape `doc/style.md` into patterns that name no surface, and
  `doc/app` into Activities, Shared and one section per surface (`documents.md`
  › Dependencies, The set); add the trace command (`documents.md` ›
  Traceability) and, where `doc/style.md` exists, the style page
  (`style-preview`).
- 6 → 7: split each test file that holds several themes (`software-design.md` ›
  Organize tests for findability). The PreToolUse hook comes with the settings
  merge.
- 7 → 8: none; the `deny` rules come with the settings merge.
- 8 → 9: none.
- 9 → 10: move the rows about the harness from `doc/deferred.md` into
  `.claude/harness/deferred.md`.
- 10 → 11: none; the harness now has a source and copies (Source and copies).
- 11 → 12: give each technology that `doc/setup` or `doc/architecture`
  documents its own parts, splitting a document that holds several
  (`documents.md` › Technology), and onboard each one the factory's kb holds
  (`tech` › Onboard a technology). Text an entry covers becomes that entry,
  heading unchanged and values filled. Everything else stays, as the project's
  own sections; a trap or pattern among it that would hold in any project
  becomes an entry.
