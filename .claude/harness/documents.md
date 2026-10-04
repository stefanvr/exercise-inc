# Documents

What goes in each project document, how documents are written, and how they
grow. Who owns which question is in `.claude/CLAUDE.md` › Where truth lives.

Create a document when the first rule it owns is decided. Do not create empty
skeletons; the sections below are the shape a document takes as it fills, not a
form to complete.

## The set

### `README.md`

```markdown
# <Product>

<What it is and for whom, 1-3 lines.>

## Running it

<the standard commands: install, run, test/check>

## Development affordances

<every URL, fixture, flag or script the standard commands do not reveal:
 one line each, what it is for and how to reach it>

## Where truth lives

| question | answer lives in |
|---|---|
| ... | `doc/...` or a source file |
```

The index lists every owning document, plus any source file that is the owner
of a decision (a token file for the palette, a module for a rule). It points; it
never restates.

### `doc/product.md`

```markdown
# <Product>

## Intent
What this is, for whom, and why it is worth building. 2-5 lines.

## Scope
What the product covers now, and what it deliberately does not.

## Decided
Product meaning that no other document owns: principles and constraints a user
feels across the whole product.
```

### `doc/domain.md`

```markdown
# Domain

## Language
| term | meaning |
|---|---|

## Rules
<per area of the domain: the rules that hold>

## Data
<what is recorded, what identifies it, what survives and for how long>
```

A term is settled when it survives the rules, data and surfaces that use it, not
when it is first named. Add events, lifecycles or other sections only when the
domain has them.

### `doc/app.md`

```markdown
# App

## Activities
What a user can do, as end state.

## Surfaces
Each screen, panel, dialog, overlay or mode: what lives there, what persists,
how a user moves between them.

## Interaction
Per surface or shared pattern: controls, gestures, feedback, and the wording a
user reads.
```

Structure only. Spacing, colour, type and sound character belong in
`doc/style.md`. Domain rules are referenced by their term, not restated.

### `doc/style.md`

```markdown
# Style

## References
Per sense, what it should feel like, and the references it is measured against.

## Visual
Rules for palette, type, spacing, motion, and states.
Values live in <the token source>.

## Audible
Per trigger (a domain event or interaction, by its term): whether it sounds,
its character, variations, and deliberate silence.
Definitions live in <the sound source>.
```

Visual and audible references may come from different sources. Style owns what
is perceived, not what causes it.

### `doc/architecture.md`

```markdown
# Architecture

## Stack
| concern | choice | why — what it beat |
|---|---|---|

## Structure
Where code lives, the boundaries between parts, and what may depend on what.

## Rules
Project-specific technical rules, each with its reason, including every
departure from `.claude/harness/software-design.md`.

## Verification
The check layers, what each proves, and the command that runs them.
```

Commands live in `README.md`; architecture points at them.

### `doc/setup.md`

```markdown
# Setup

## Development
Prerequisites with versions; bootstrap; how to confirm it works.

## Runtime
Environments; how a build is deployed; external services and their
configuration; environment variables and secrets by name, never by value; how
to confirm it is running correctly.

## Manual steps
### <step>
- who:      <the user, or a named role>
- why:      <privilege | secret | provider console | device | account | authority>
- action:   <exact command or exact clicks>
- when:     <once per machine | per environment | per release>
- expected: <the state afterwards>
- verify:   <the command or observation that shows it worked>
```

### `doc/deferred.md`

```markdown
# Deferred and rejected options

| date | option | status | reason |
|---|---|---|---|
| 2026-01-31 | <the option, named so a later reader recognises it> | rejected | <why> |
| 2026-01-31 | <the option> | deferred | <why, and the condition to revisit> |
```

One table for every concern. Replace a row when its option is later taken up;
the owning document then states it as decided.

### `doc/discovery/`

Supplied material — briefings, notes, references, screenshots — kept as
supplied. It explains intent; it is not authority and may be wrong on details or
order. Where it is incomplete for the current work, say what is missing, and add
what the user supplies. Accepted content is written into the owning document;
discovery material is not maintained to match later decisions.

## Writing

- **End state.** State the rule that holds, or the position to build toward. No
  "was/now", no "we decided", no progress narrative, no rejected alternatives.
- **Rule, not instance.** "Continents meet only at narrow necks" is a rule.
  "Thirty territories in six continents" is an instance: it belongs in the code
  and in a check that asserts it. A copied instance goes stale in silence.
- **Without technology** in `product`, `domain`, `app` and `style`. What is left
  there is what would still be true on a different stack.
- **One vocabulary.** Use the terms in `doc/domain.md` › Language unchanged.
  Introduce a term there before using it elsewhere.
- **Decided wording is quoted exactly**, in the document that owns the surface.
- **Pointers, not copies.** When another document owns a rule, name it and
  point; do not restate it.
- **Elaborate where it changes understanding:** surprising rules, exceptions,
  boundaries that are easy to misuse. Do not explain the obvious.

## Growth

When a document passes about 300 lines, or holds parts that change for different
reasons, turn `doc/<name>.md` into a directory:

- `doc/<name>/index.md` — the rules common to all parts, and the list of parts;
- `doc/<name>/<part>.md` — one per area of the product: a domain area, a
  surface, a sense. Never one per feature, work item or date.

Move the text; leave no stubs. Update the README index and every pointer.

## Markers

| marker | meaning | listed at review |
|---|---|---|
| `PROVISIONAL: <what is open>` | a default taken without the user | every review, until settled or deferred |
| `DEFERRED: <what> — doc/deferred.md` | the user postponed it; the default stands | no |

Place a marker beside the value: in a code comment where the code holds it,
inline in a document where a document calls it open. Both, when both apply.

- Settling one: write the decision in the owning document, set the value,
  remove the marker.
- Deferring one: replace `PROVISIONAL` with `DEFERRED`, and add a
  `doc/deferred.md` row with the condition to revisit.

## Traceability

- A test that proves a specified rule names it: its name or enclosing group
  names the owning document and section, in the domain's terms. For example
  `domain › Reinforcements: a player holding a continent receives its bonus`.
- Every specified behaviour has a check: an automated test, or a named manual
  inspection in `WORK.md` › Done when.
- A test asserting product behaviour with no owning section is a missing spec,
  not a free test.
- Passing tests prove only the requirements they encode.

## `WORK.md`

The work in progress on the current branch. A local file, never committed:
`.gitignore` lists it. It survives sessions on this machine, not a move to
another; it is deleted at landing.

```markdown
# Work: <one line>

Branch:    work/<slug>
Why:       <the request, quoted where possible, or the gap it closes>
Spec:      pending | approved <date> | not needed — <reason>
Done when: <the evidence: the commands and what they must show; what is
           inspected on the surface, and how>

## Plan
- [ ] <only steps whose state must survive a session>

## Next
- <requests the user made during this work that belong to later work, in order>
```

`WORK.md` holds no decisions. A decision goes into the document that owns it,
the moment it is made.
