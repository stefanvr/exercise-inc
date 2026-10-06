# Domain

## Language

| term | meaning |
|---|---|
| timer | A saved interval timer: an optional start delay, then a number of repeats of work and rest. |
| settings | The values that make a timer: start delay, work, rest and repeats. |
| start delay | The countdown before the first work, to get into position. Optional. |
| work | The phase in which the athlete exercises. |
| rest | The phase that follows each work. |
| repeat | One work followed by one rest. "Repeats" is the number of them in a timer. |
| phase | One countdown within a started timer: the start delay, a work or a rest. |
| name | How a timer is shown to the user, generated from its settings. |

## Rules

### Settings

- Durations are whole seconds; repeats is a whole number.
- A timer has either no start delay or a start delay of at least 5 s.
- Work and rest are at least 1 s; repeats is at least 1.
- Each value has an upper bound, so that a phase fits the `m:ss` countdown; the
  bounds are in the code.
- No two timers have the same settings. A timer is its settings, so a copy
  would add nothing.
- Settings do not change once a timer is created. DEFERRED: editing a timer —
  `doc/deferred.md`.

### Name

The name is `{repeats} × {work}/{rest} s`, followed by `· {start delay} s
delay` when the timer has a start delay: `8 × 30/5 s · 10 s delay`,
`8 × 30/5 s`. Durations stay in seconds above a minute: `3 × 90/30 s`.

### Starting a timer

- A started timer runs its phases in order: the start delay, if it has one, then
  work and rest for each repeat. The last repeat ends with its rest, like every
  other.
- A started timer is running, paused or done.
  - **Running**: the current phase counts down. Pause makes it paused; End stops
    it.
  - **Paused**: the countdown holds. Resume continues from the same moment.
    Restart starts it again from its first phase and makes it running. End stops
    it.
  - **Done**: the last rest has run out.
- The countdown follows real elapsed time. It goes on while the app is in the
  background or the screen is off.

## Data

- Timers stay on the phone until the user deletes them. They survive closing
  the app and restarting the phone.
- A started timer is not recorded. When the app is closed, a started timer is
  gone; the app reopens on the timer list.
