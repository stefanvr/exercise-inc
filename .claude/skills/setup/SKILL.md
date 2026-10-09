---
name: setup
description: Use when something outside the code must be prepared (tools, accounts, services, secrets, deployment, manual steps) or doc/setup.md changes.
---

# Setup

`doc/setup.md` lets a person bring a fresh machine to a working development
state, and the product to a running state, without asking anyone. Its shape is
in `.claude/harness/documents.md`. Steps for one technology go in its part,
`doc/setup/<tech>.md`, as entries (`documents.md` › Technology).

## Write from evidence

- Write a step from what happened when it was run — by you, or by the user with
  the result seen — not from what the technology's documentation says usually
  happens. A step not yet run says so.
- Verify against the resulting state — the service answers, the page loads, the
  variable is visible — not against a tool reporting success.
- Every manual step records who, why it is manual, the exact action, when it is
  needed, the expected state, and how to verify it.

## Manual boundaries

A step that needs privilege, a secret, a provider console, a device, an account,
or authority you do not have is the user's.

- Never claim such a step was done because you described it.
- Ask the user to do it. For an interactive command, suggest they type
  `! <command>` so its output lands in the session.
- Then verify the resulting state yourself.
- Never write a secret value into the repository. Name the secret and where it
  is kept.

## Split with README

`README.md` carries the everyday commands and the dev affordances; `doc/setup.md`
carries preparing environments. Point from one to the other; do not repeat.
