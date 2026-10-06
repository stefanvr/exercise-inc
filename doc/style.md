# Style

## References

None yet. The timer list and the New timer page keep the platform's default
look. DEFERRED: the look — `doc/deferred.md`.

## Visual

Values live in `src/ExerciseInc/Resources/Styles/Colors.xaml`.

- **Phase colours**, as the timer screen's background: start delay blue, work
  red, rest green.
- **Last seconds.** During the last 5 s of each phase, the whole background
  flashes once a second between the phase's colour and a light version of it. A
  phase of 5 s or less flashes throughout. A paused timer does not flash.
- **Paused**: the phase's colour, dimmed.
- **Done**: a neutral dark background.
- The countdown is the largest element on the timer screen, readable from a few
  metres away. The phase is shown in capitals.

## Audible

The app is silent. DEFERRED: sound for timers — `doc/deferred.md`.
