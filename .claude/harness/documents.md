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

A rule in another document that applies a principle points to it
(`doc/product.md` › Decided) and does not restate it.

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
| activity | what the user does, as end state | surface |
|---|---|---|

## Shared
What every surface follows, stated once: the frame around the surfaces, moving
between them, and the states they share, such as signed out, waiting,
unreachable or failed.

## <Surface>
One section per screen, panel, dialog, overlay or mode: what lives there, what
persists, its controls, gestures and feedback, which style pattern each part
uses, and the wording a user reads. Only what is its own; a shared rule is not
restated.
```

Structure only. A sketch shows what is on a surface and in what order, not how
it looks. How a pattern looks, and every sound, belong in `doc/style.md`;
which sound an interaction makes, or that it stays silent, belongs here. Domain
rules are referenced by their term, not restated.

### `doc/style.md`

```markdown
# Style

## References
Per sense, what it should feel like, and the references it is measured against.

## Visual
Values live in <the token source>; <the style page> shows every token and
pattern.

### <Pattern>
One section per pattern — palette, type, spacing, controls, lists and tables,
the frame, motion, states: the rules that hold wherever the pattern is used.

## Audible
Definitions live in <the sound source>.

### <Sound>
One section per sound, by its role: its character and variations.
```

Style names no surface (Dependencies): a rule that holds on one surface only is
a pattern that surface uses, and `app` names the use. A token is named as the
token source names it, and its value is left there. Visual and audible
references may come from different sources. Style owns what is perceived, not
what causes it.

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

One table for every concern of the project; the harness keeps its own
(`.claude/CLAUDE.md` › Changing the harness). Replace a row when its option is later taken up;
the owning document then states it as decided.

### `doc/discovery/`

Supplied material — briefings, notes, references, screenshots — kept as
supplied. It explains intent; it is not authority and may be wrong on details or
order. Where it is incomplete for the current work, say what is missing, and add
what the user supplies. Accepted content is written into the owning document;
discovery material is not maintained to match later decisions.

## Dependencies

The documents depend in one direction. A document points at what it depends on
and never at what depends on it.

| document | depends on |
|---|---|
| `product` | nothing |
| `domain` | `product` |
| `style` | `product` |
| `app` | `product`, `domain`, `style` |
| `architecture`, `setup` | all of the above |

Every document uses the terms of `doc/domain.md` › Language; that is the
vocabulary, not a dependency.

The test of placement: a different `style` restyles the product without a
change to `app`, and a different `architecture` rebuilds it without a change to
the product documents. A rule that fails the test is in the wrong document. Read
together, the documents are enough to build the product again.

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
- **Stand alone.** A spec document's rules, values and reasons are readable
  from this repository alone. What another project settles for this one is
  brought in: its substance into the owning document, its assets into the
  repository. Links to the documentation of the technology used, to standards
  and to external services stay links.
- **Elaborate where it changes understanding:** surprising rules, exceptions,
  boundaries that are easy to misuse. Do not explain the obvious.

## Growth

When a document passes about 300 lines, or holds parts that change for different
reasons, turn `doc/<name>.md` into a directory:

- `doc/<name>/index.md` — the rules common to all parts, and the list of parts;
- `doc/<name>/<part>.md` — one per area of the product: a domain area, a
  surface, a sense. Never one per feature, work item or date.

A part holds only its own rules; one common to all parts is stated in
`index.md` and not repeated in a part. In `doc/app/`, `index.md` holds
Activities and Shared, and the Activities table names each surface's part: it
is the list of parts.

Move the text; leave no stubs. Update the README index and every pointer.

## Technology

What the project knows of a technology it uses sits in a part per technology:
the steps that prepare it in `doc/setup/<tech>.md`, the behaviour code and
checks must respect in `doc/architecture/<tech>.md`. The first such part turns
its document into a directory (Growth). Knowledge goes in the part of the
technology it is about, not of the one the work was on; a trap met while doing
a setup step stays in that step.

These parts are written so the next project can take them in: `tech` › Onboard
a technology fills them from the factory's kb, and the factory harvests them
back. Each `##` section is an entry or a project rule. An entry's heading
states its claim and names no project part; it is the entry's identity in
every project, so an onboarded entry keeps it word for word.

**Issue**: something that behaves other than it appears.

```markdown
## <the claim, stated as a fact>
Seen: <project>, <date>, <tool> <version>

- **What happens.**
- **Why it misleads.**
- **Safe pattern.**
- **Verify.**
- **Stops applying.**
```

**Work**: a pattern that proved itself.

```markdown
## <the pattern, stated as a claim>
Seen: <project>, <date>, <tool> <version>

- **Pattern.**
- **Verify.**
- **Stops applying.**
```

**Setup step**: a manual step as in `doc/setup.md` above, as a `##` section,
with no `Seen:`.

- `Seen:` says where and when the entry last held, with which versions. A later
  sighting, here or elsewhere, replaces it.
- An onboarded entry's placeholders take this project's values; its heading
  stays.
- A project's own rule about the technology — a choice, a value, a boundary —
  is a section of its own, without `Seen:`, never text inside an entry.
- A trap or pattern found here that would hold in any project on the
  technology is a new entry, with `Seen:`. One that holds only here is a
  project rule.

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
- The project has a trace command, listed in `README.md`. It reports, for each
  section of `domain`, `app` and `style`, the tests that name it; the sections
  no test names; and the tests that name no section, or one that does not
  exist. `present` shows it for the sections changed on the branch; `sanity`
  runs it whole. A section no test names is checked by a named manual
  inspection, or it is a gap.

## `WORK.md`

The work in progress on the current branch. A local file, never committed:
`.gitignore` lists it. It carries the work across sessions on this machine —
each phase boundary ends one (`.claude/CLAUDE.md` › The loop) — not a move to
another; it is deleted at landing.

```markdown
# Work: <one line>

Branch:    work/<slug>
Phase:     specify | build | present | land
Why:       <the request, quoted where possible, or the gap it closes>
Spec:      pending | approved <date> | not needed — <reason>
Landing:   pending | approved <date>
Done when: <the evidence: the commands and what they must show; what is
           inspected on the surface, and how>

## Plan
- [ ] <only steps whose state must survive a session>

## For review
- <what `present` reports that the repository does not show: each delegation
  (kind, model, what came back); each learning to propose, with the document
  that would own it>

## Next
- <requests the user made during this work that belong to later work, in order>
```

`WORK.md` holds no decisions beyond the gates' state in Spec and Landing. A
decision goes into the document that owns it, the moment it is made.
