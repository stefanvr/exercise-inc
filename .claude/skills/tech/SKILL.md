---
name: tech
description: Use for a stack-level technology choice (language, framework, persistence, hosting, test tooling), a technical proof, or a technical rule.
---

# Tech

Stack-level choices are the user's. You frame the choice; the user makes it.

## Decide a technology

1. State the decision and why the work needs it now. A technology decision is
   made before the work that depends on it, never inside that work under
   delivery pressure.
2. Collect the constraints: what the product documents require (platform,
   offline use, cost, hosting, data that must survive), `doc/architecture.md`,
   known setup facts, and the user's stated preferences.
3. List the plausible candidates. Eliminate those a constraint rules out, naming
   the constraint.
4. Compare the remaining two to four on the trade-offs that matter for this
   product. Verify external claims — versions, limits, pricing, licences —
   against current sources, not memory.
5. Where real behaviour is uncertain and the choice depends on it, prove it:
   the smallest thing that genuinely exercises the uncertainty, outside the
   product code (a scratch directory or a throwaway branch), with no product
   rules in it. A proof answers the question; it is not the start of the
   product. Keep its findings, not its code.
6. Ask through the question dialog: the trade-off per option, the
   recommendation first.
7. Record it in `doc/architecture.md` › Stack: the choice, what it beat, and
   why. Add a `doc/deferred.md` row for a rejected candidate the user might
   raise again.
8. Follow the consequences:
   - setup steps → `setup`;
   - the run and check commands → `README.md`;
   - the check commands as allowed permissions in `.claude/settings.json` → a
     `p` item.

## Add a technical rule

A project-specific technical rule — module boundaries, data versioning, a
compatibility policy, a departure from `software-design.md` — goes into
`doc/architecture.md` › Rules with its reason. A rule must constrain something
real.

A rule that changes what a user experiences, such as stored data not surviving
an upgrade, is also a product decision, which is the user's.

## Minor libraries

Yours to choose. Prefer none where the platform already covers the need. Name
each one added, with its reason, in the review.
