# Harness

Version 3 — 2026-10-03.

Operating instructions for building a product with Claude Code. They live inside
the product's repository, so they load in every session mode; an `@import` from
outside the project tree does not load under `claude -p`.

## Files

| path | owner | holds |
|---|---|---|
| `.claude/CLAUDE.md` | harness | always loaded: role, communication, where truth lives, the decision rule, the loop, delegation, repository discipline |
| `.claude/harness/documents.md` | harness | what goes in each project document, writing rules, growth, markers, traceability, `WORK.md` |
| `.claude/harness/software-design.md` | harness | implementation-design preferences, read in `build` |
| `.claude/skills/*/SKILL.md` | harness | one procedure per phase or side task, loaded when used |
| `.claude/hooks/orient.sh` | harness | session orientation, run by the SessionStart hook |
| `.claude/settings.json` | shared | the SessionStart hook, the baseline git permissions and `autoDreamEnabled: false` (learnings go to project documents, not to memory) belong to the harness; the project adds its own check commands |
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

## Using it in a new project

Copy `.claude/` without `settings.local.json` into the repository root, make
`.claude/hooks/orient.sh` executable, add `WORK.md` to `.gitignore`, and start
a session. Without
`doc/product.md`, the first work establishes the product's intent.

## Upgrading

Copy the harness-owned files over the project's. Merge `.claude/settings.json`
by hand, keeping the project's permissions. From version 2 to 3: add `WORK.md`
to `.gitignore`. Projects never edit harness-owned
files: a project rule goes into the project document that owns its subject. A
harness improvement found in a project is made in the harness, raises the
version above, and is then copied.
