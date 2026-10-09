---
name: style-preview
description: "Use when doc/style.md, the tokens or the sounds change, or no page shows them yet: a dev page showing every style pattern from its real source."
---

# Style preview

Make every style decision directly inspectable, without driving the product into
the state where it appears. The preview demonstrates `doc/style.md`; it decides
nothing. With it, the style guide is three views of one thing and no copy: the
rules in `doc/style.md`, the values in the token source, and the page that shows
both.

## Build

- A dev-only page, or one per sense if they grow apart, with one section per
  pattern of `doc/style.md`, under the same name and in the same order:
  - **visual**: every token with its name, its role and its value — palette,
    type, spacing, motion — and each pattern drawn with the real components
    in their states (default, hover, focus, active, disabled, error);
  - **audible**: every sound by its role, with a play control and its
    variations.
- Read the values from their real sources: the token file, the sound
  definitions, the real components. Never copy a value into the preview; a copy
  hides drift.
- Prefer systematic, tabulated presentation over recreating product screens.
  Surfaces in context are shown by the product or its dev scenes, not rebuilt
  here.
- Gate it from production builds, keep it reachable in development and by the
  tests, and list its URL in `README.md`.
- Add a check that loads the preview and asserts that every token in the token
  source and every sound definition appears, and that every pattern and token
  `doc/style.md` names has its place on the page.

## Use

- While a style decision is being made, show the alternatives — `s1`, `s2` — in
  the preview or in the product, side by side where feasible, before asking.
- When `doc/style.md` or a style source changes, update the preview in the same
  work.
- Name in the review any style decision that cannot be demonstrated yet.
