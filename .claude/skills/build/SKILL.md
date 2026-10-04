---
name: build
description: "Loop phase Build. Use after the spec is settled: define the checks that prove it, implement in small commits, delegate bounded parts."
---

# Build

## Preconditions

- On the work branch. `WORK.md` › Spec is approved or not needed; trivial work
  has no `WORK.md`, and its check is stated in the conversation instead.
- No stack-level choice is open: `doc/architecture.md` settles it, or run `tech`.

Otherwise return to `specify` or `tech`.

Silence in the documents is not permission. Product behaviour the documents do
not state is a decision under the decision rule, not an implementation detail.

## 1. Define verification

Decide how the result will be demonstrated before implementing it, and make sure
`WORK.md` › Done when states it.

- Prefer repeatable checks. Tests are part of the implementation, not cleanup.
- Prove behaviour at the lowest truthful layer (`software-design.md` ›
  Testing). Verify user-visible behaviour at the surface wherever a lower layer
  cannot show that the product is right.
- A check crosses the boundary the work was described as crossing. Work put to
  the user as two browsers, two processes or two machines is not proven
  in-process. Narrowing the promise between description and check is a scope
  change, which is the user's.
- A user-visible change is checked with evidence from the surface as a user
  meets it — rendered output, a screenshot, an observed interaction — not only
  an exit code. A test that an attribute is set is not evidence that a person
  can perceive the change.
- A change a user meets is not done until the surface says what happened. Where
  a transition is subtle, the work includes the words that confirm it.
  Information the domain produces and the surface discards is a defect.
- Where the claim is connectivity or integration, the deliverable includes how
  the user reaches it: a URL, a command, a written procedure.
- Name each test after the behaviour and the owning document section
  (`documents.md` › Traceability).

Where useful, write the check first and show it failing. Commit:
`Check <the change>`.

## 2. Implement

Read `.claude/harness/software-design.md` before writing code in a session, and
apply it unless `doc/architecture.md` has a more specific rule.

- Implement against the approved end state, not against the existing
  implementation.
- Small commits, one responsibility each. Correct a commit already on this
  branch with `git commit --fixup=<sha>`.
- A new decision: apply the decision rule. One that changes approved meaning
  stops the work and goes to the user now. One that only fills a gap takes a
  `PROVISIONAL` default and goes to the review.
- A new stack-level technology: stop, run `tech`.
- A tool, dependency, service, account, environment variable or manual action
  the project now needs: run `setup` in the same work.
- A technical rule this work establishes, or a departure from
  `software-design.md`: write it into `doc/architecture.md` in the same work.
- A dev affordance (fixture, preview page, debug entry point): gated from
  production, reachable by the tests, and listed in `README.md` when created.
- Keep `WORK.md` › Plan current where a session may end mid-work.

## 3. Delegate

A writing sub-agent receives exactly this, and nothing it must guess:

```
Responsibility:  <one thing>
Paths:           <the only paths it may change>
Constraints:     <the document rules that apply, quoted or pointed at>
Expected result: <what exists afterwards>
Check:           <the command that must pass; it fails now>
Report:          a summary of the diff, the check output, and every decision
                 you met and did not make. Make no user-visible decision.
```

Choose its model per call (CLAUDE.md › Delegation). Inspect the diff and re-run
the check yourself before building on the result.

## Done

Every check in `WORK.md` › Done when passes, run by you. Continue with
`present`.
