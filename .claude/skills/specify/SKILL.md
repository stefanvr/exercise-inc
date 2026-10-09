---
name: specify
description: "Loop phase Discover/Specify. Use first for any product change: turn the request into an end state in the owning docs, approved before anything is built."
---

# Specify

Turn a request into an end state written in the owning documents, precise enough
to build and to check, and approved by the user where the spec gate applies.

Do not turn a non-trivial request directly into implementation. Do not specify
further than this work needs: a rule written ahead of the work is written from a
worse understanding. A request larger than one slice: propose the smallest slice
that runs end to end and confirm it with the user before asking about its
details; later features wait for their own work.

## 1. Discover

Read-only; allowed before the branch exists. Read only what the request touches:

1. `README.md` and its index;
2. `doc/product.md`, the owning sections of `doc/domain.md`, `doc/app.md`,
   `doc/style.md`, and `doc/deferred.md`;
3. `doc/architecture.md` where the change has technical consequences;
4. the relevant code and tests, and `git log` for the area;
5. `doc/discovery/` material on the subject.

Use analysts for wide surveys. Establish:

- the current behaviour, observed, with `file:line`;
- the requested outcome;
- the affected surfaces and domain concepts;
- each decision the request leaves open, classified: settled (and where) /
  yours / the user's, bounded / the user's, open;
- any stack-level technology the work needs that `doc/architecture.md` does not
  settle — run `tech` before going further;
- the evidence that will prove the change.

The request and the discovery material are a starting point, not a complete
specification. Where discovery material is incomplete for this work, say what is
missing.

**A new product** (no `doc/product.md`): ask only enough to write its Intent and
choose the first work. What it is, for whom, and why are open questions: ask
them as such (`.claude/CLAUDE.md` › Question dialog). Propose the first work as
the smallest slice (above). Write `README.md` and `doc/product.md` as the first
spec.

## 2. Open

1. From `main`: `git switch -c work/<slug>`.
2. Unless the work is trivial, write `WORK.md` (template in
   `.claude/harness/documents.md`) with `Phase: specify`, `Spec: pending` and
   `Landing: pending`.

## 3. Language

For domain-heavy work, settle the vocabulary first. Each concept the work names
gets one term in `doc/domain.md` › Language, used unchanged in documents, code,
tests and UI text. A term you propose is a terminology decision, which is the
user's. Revise a term when using it in rules, data or surfaces exposes a
mismatch.

## 4. Refine with the user

Apply the decision rule to each open decision.

- **Bounded**: the question dialog, rounds of up to four.
- **Open**: the question dialog with unranked sketches — `s1`, `s2` — as
  options, and let the user originate in its free text.
- **Anything with a surface**: show a sketch before writing the rule — an ASCII
  layout, a mockup, or a screenshot of the current state with the change marked.
  Reacting to a picture is cheaper than reacting to prose.
- **A visual or audible detail**: decide it as a pattern in `doc/style.md`, or
  use one already there, so every surface that uses the pattern has it
  (`documents.md` › Dependencies).

The spec covers, where relevant: product behaviour; domain rules and vocabulary;
workflow and interaction; visual and audible behaviour; data and persistence,
including what survives; error and edge behaviour; acceptance evidence.

## 5. Write the end state

1. Write each accepted decision into the document that owns it, as the rule
   that will hold once built. Follow `.claude/harness/documents.md`.
2. Rejected options become `doc/deferred.md` rows.
3. Acceptance evidence goes into `WORK.md` › Done when.
4. Check the spec internally: the touched sections agree with each other and
   with the rest of the documents, use the same vocabulary, define compatible
   behaviour, and keep the direction of `documents.md` › Dependencies.
5. Commit: `Specify <the change>`.

## 6. Gate

The gate applies when the work involved any user-visible decision not settled
before it started. Then:

1. summarise the end state in product terms, labelled, in 5-15 lines;
2. point at the full diff: `git diff main -- doc/`;
3. list every default taken (`PROVISIONAL`);
4. ask through the question dialog: approve, or change (and what).

On approval set `Spec: approved <date>`. On a change, revise and ask
again. When the gate does not apply, set `Spec: not needed — <reason>`.

Trivial work continues with `build` in this session. Otherwise set
`Phase: build` and end the session (`.claude/CLAUDE.md` › Phase boundaries).

## Requirement changes

When the user changes what the work means — during build, or at review — return
here. Update the owning documents first, in a new commit that is never folded.
Re-run the gate if the change involves a user-visible decision the user did not
state completely. Then continue where the work was.
