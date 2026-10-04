---
name: style-preview
description: "Use when a visual or audible style decision is made or changes: dev-only pages that show every style token and sound from its real source."
---

# Style preview

Make every style decision directly inspectable, without driving the product into
the state where it appears. The preview demonstrates `doc/style.md`; it decides
nothing.

## Build

- A dev-only page, or one per sense if they grow apart:
  - **visual**: every token with its name and role — palette, type scale,
    spacing, motion — and the states of shared components (default, hover,
    focus, active, disabled, error);
  - **audible**: every specified sound with a play control, its trigger by its
    term from `doc/domain.md` or `doc/app.md`, its variations, and the
    deliberate silences listed.
- Read the values from their real sources: the token file, the sound
  definitions, the real components. Never copy a value into the preview; a copy
  hides drift.
- Prefer systematic, tabulated presentation over recreating product screens.
- Gate it from production builds, keep it reachable in development and by the
  tests, and list its URL in `README.md`.
- Add a check that loads the preview and asserts that every token in the token
  source and every sound definition appears.

## Use

- While a style decision is being made, show the alternatives — `s1`, `s2` — in
  the preview or in the product, side by side where feasible, before asking.
- When `doc/style.md` or a style source changes, update the preview in the same
  work.
- Name in the review any style decision that cannot be demonstrated yet.
