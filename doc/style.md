# Style

## References

None yet. DEFERRED: the look — `doc/deferred.md`.

## Visual

Colours live in `src/ExerciseInc/Resources/Styles/Colors.xaml`, the type and
buttons drawn on colour in `TimerScreen.xaml` beside it; the style preview shows
every token and pattern (`README.md` › Development affordances).

### Phase colours

A started timer's background has the colour of its phase: start delay blue,
work red, rest green.

### Last seconds

During the last 5 s of each phase, the whole background flashes once a second
between the phase's colour and a light version of it. A phase of 5 s or less
flashes throughout. A paused timer does not flash.

### Paused

The background has the phase's colour, dimmed.

### Done

A neutral dark background.

### Countdown

The largest element on its screen, readable from a few metres away.

### Phase label

The phase, in capitals.

### On colour

On the phase colours and the done background, text is white, and a button is
white text in a white outline over a faint white fill.

### Error text

Text that says why a value is not accepted is red.

## Audible

None yet. DEFERRED: sound for timers — `doc/deferred.md`.
