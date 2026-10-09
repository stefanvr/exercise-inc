---
name: sanity
description: "Use for an on-demand consistency audit: docs vs docs, docs vs code and tests, repository hygiene, harness. Reports findings, fixes nothing."
---

# Sanity

Find drift; do not fix it here. Each finding is `f<n>`: what is wrong, the
evidence (`file:line` on both sides), the owning document, and a suggested
resolution. Unknown is not success: say what could not be checked.

Run the four checks with analysts in parallel — a cheap model, read-only — each
returning findings with evidence. Confirm each finding yourself before reporting
it.

## 1. Documents, internally

- Every decision has one owner. No rule is stated in two places, with the same
  content or different. Every pointer resolves.
- Dependencies (`documents.md` › Dependencies): no document points at one that
  depends on it; `domain` and `style` name no surface; a part repeats no rule
  of its `index.md`; `product` › Decided is pointed at, not restated.
- Vocabulary: the terms in `doc/domain.md` › Language are used unchanged
  elsewhere. No synonyms for one concept; no undefined term.
- Product documents name no technology. `doc/architecture.md` states no product
  rule except by pointer.
- End state only: no "was/now", no rejected options, no progress narrative, no
  instances copied from code.
- No option in `doc/deferred.md` is stated as decided elsewhere.
- No document is past the growth limit (`documents.md` › Growth).
- In a technology's part, every issue and work entry has a `Seen:` line, and no
  entry's heading names a project part (`documents.md` › Technology).

## 2. Documents, code and tests

- Run the trace command (`documents.md` › Traceability); its gaps are
  candidate findings.
- Spec → tests: every rule in `domain`, `app` and `style` has a check or a
  named manual inspection, beyond its section having a test.
- Tests → spec: every test asserting product behaviour names an owning section
  that exists and still says that.
- Code → spec: behaviour in code with no owning rule (orphan behaviour); rules
  with no implementation (orphan spec).
- UI wording in the code matches the decided wording in `doc/app.md`.
- Markers: an open decision a document calls open is marked in the code too,
  and the other way round. Every `DEFERRED` marker has a `doc/deferred.md` row.

## 3. Repository

- `README.md`: the commands run; the listed affordances exist and are
  reachable; every index entry exists.
- `doc/setup.md` matches the project: versions, scripts, environment names.
- Dev affordances are gated from production.
- Where `doc/style.md` exists, the style page exists and shows every token and
  pattern it names (`style-preview`).
- `WORK.md` is in no commit and absent on `main`; no stale `work/` branches,
  no leftover proof code.

## 4. Harness

- The harness files — `.claude/CLAUDE.md`, `.claude/harness/`,
  `.claude/skills/`, `.claude/hooks/` — contain no project-specific truth.
- They agree with each other: phases, skill names, file names, markers.

## Report

Findings grouped by owning document, most consequential first. Then apply the
decision rule to each resolution: a fix fully settled by the documents is
trivial work on a branch; anything else is an `n` item.
