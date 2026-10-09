---
name: present
description: "Loop phase Verify/Review. Use when checks pass: verify yourself, show the working product, report in fixed shape, put decisions to the user."
---

# Present

## 1. Verify, yourself

1. Run every check in `WORK.md` › Done when, and the project's full check
   command from `README.md`. Read the output.
2. Inspect the product as a user meets it: run it, take screenshots, play the
   sound, open the URL. Where a surface is involved and you cannot inspect it,
   say so: it is not verified.
3. Compare the result with the spec — `git diff main -- doc/` and the owning
   sections — not with the implementation.
4. Check that the tests encode the specified behaviour: run the trace command
   (`documents.md` › Traceability) and read, for each section changed on the
   branch, the tests that name it against its rules. Every rule changed in the
   spec has a check, and no passing test is taken as proof of a requirement it
   does not encode.
5. Check the repository: `git status`, `git diff main --stat`,
   `git log --oneline main..HEAD`. Only intended changes, no stray files.
6. List the markers: `git grep -n PROVISIONAL -- ':!.claude'`.
7. Check that `README.md`, `doc/setup.md` and `doc/architecture.md` still
   describe the result.

Fix what fails before presenting, back in `build`.

## 2. Show first

Open with the product as a user meets it: a URL, the command to run, a
screenshot, the page. The user's reaction to what they can see is worth more
than their reaction to a description, and it is where the next work comes from.

## 3. Report

Exactly this shape. Write `none` for an empty section.

```
Changed        what changed, in product terms
               each dependency added, with its reason
               each delegation: kind, model, what came back
Verified       each check re-run, with its actual result;
               what was inspected on the surface, and how
Traced         each section changed on the branch, with the tests that name
               it, or its manual inspection, or `no check`
Decisions      n1 … every open user-visible decision, including every
               PROVISIONAL marker a user would notice, at every review
               until it is settled or deferred
Documentation  the documents changed to match the result
Repository     the commits that would land
Proposes       p1 … learnings, each with the document that would own it;
               document revisions where reality contradicts stated intent;
               harness changes
```

Delegations and learnings from earlier sessions are in `WORK.md` › For review.

Propose a revision of a product document whenever reality contradicts it: what
was specified turns out wrong, expensive or pointless. Name the contradiction,
cite the evidence, propose the specific edit.

## 4. Ask

Put the `n` and `p` items through the question dialog, rounds of up to four:
recommendation first for bounded items, unranked sketches for open ones. Then ask for landing: land now,
or change first.

| answer | action |
|---|---|
| changes what the work means | `specify` › Requirement changes |
| reports a defect, or asks a correction it settles completely | a small fix: here (below); otherwise a `WORK.md` › Plan step, back to `build` |
| settles a marker | record it in the owning document now; remove the marker |
| defers a marker | `DEFERRED` marker and a `doc/deferred.md` row |
| accepts a proposal | write it into its owning document on this branch |
| rejects a proposal | a `doc/deferred.md` row; for a harness change, a `.claude/harness/deferred.md` row |
| asks for something beyond this work, such as a new activity | `WORK.md` › Next; this work lands first |
| approves landing | `Landing: approved <date>` |

**A small fix here.** A fix is small when the answer leaves no decision open and
it adds no new structure. Make it in this session, as `build` › Implement does:
a rule the answer adds goes into its owning document first, then the check, then
the code. Re-run the checks it touches and the full check command, and show
again only what changed. A fix that turns out larger goes back to `build`.

Trivial work lands in this session on the user's word. Otherwise, once every
answer is recorded and every small fix shown, set Phase — `land` when landing is
approved, else the phase the answers send the work back to — and end the session
(`.claude/CLAUDE.md` › Phase boundaries).
