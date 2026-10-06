# App

## Activities

- Create a timer from its settings.
- Delete a timer.
- Start a timer and follow it full screen; pause it, resume or restart it, and
  end it.

## Surfaces

### Launcher

The app's icon on the phone, labelled with the app name.

### Timer list

The page the app opens on. The title bar shows the app name and New. Below it,
one row per timer showing its name, in the order the timers were created,
newest last. At the bottom, Start and Delete, which act on the selected timer.

### New timer

Opened by New. A field per setting, the name the values make, Cancel and Save.
Both buttons return to the timer list.

### Timer screen

Opened by Start. It fills the whole screen: no title bar, and the phone's status
and navigation bars are hidden. It shows the started timer as running, paused or
done. End and the end of the Done state return to the timer list.

## Interaction

### Timer list

- Tapping a row selects it; one timer is selected at a time. Start and Delete
  are disabled while none is.
- Delete asks for confirmation. Confirming removes the timer; cancelling keeps
  it.
- After Save, the new timer is in the list and selected.
- Without timers, the list shows the empty-list text.

### New timer

- Fields use the numeric keypad. The start delay may be left empty for no
  delay; the other fields start empty.
- The name line shows the name while all values are valid.
- Save is disabled while a value is out of its range or the settings match an
  existing timer. A field out of range shows its allowed range below it; a
  match shows the duplicate text.

### Timer screen

- **Running**: the phase, the countdown, the repeat line, Pause and End. The
  background has the phase's colour and flashes during its last seconds
  (`doc/style.md`).
- The countdown shows the time left in the phase as `m:ss`, rounded up to the
  whole second: a 30 s work starts at `0:30` and ends as `0:01` runs out.
- The repeat line shows the current repeat and the number of repeats. It is
  hidden during the start delay.
- **Paused**: the paused label above the phase, the countdown held, the repeat
  line, Resume, Restart and End.
- **Done**: the done text over the whole screen. A tap anywhere returns to the
  timer list.
- The screen stays on while the timer is running.
- The phone's back gesture pauses a running timer, and ends a paused or done
  one.

### Wording

- app name: "Exercise Inc"
- timer list: "New", "Start", "Delete"
- empty-list text: "No timers yet."
- delete confirmation: title "Delete timer?", the timer's name as message,
  buttons "Delete" and "Cancel"
- new timer: title "New timer"; fields "Start delay (s)", "Work (s)", "Rest (s)",
  "Repeats"; start delay hint "empty: no delay; otherwise 5 or more"; name line
  "Name: {name}"; buttons "Cancel" and "Save"
- allowed range: "{min} to {max}"; for the start delay "empty, or {min} to
  {max}"
- duplicate text: "This timer already exists."
- phases: "Start delay", "Work", "Rest"
- repeat line: "repeat {current} / {repeats}"
- timer screen buttons: "Pause", "End", "Resume", "Restart"
- paused label: "Paused"
- done text: "Done"
