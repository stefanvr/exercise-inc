# App

## Activities

| activity | what the user does, as end state | surface |
|---|---|---|
| create a timer | A timer with the settings the user entered is in the list, selected. | New timer |
| delete a timer | After confirming, the timer is gone. | Timer list |
| run a timer | The started timer runs full screen; the user pauses it, resumes or restarts it, and ends it. | Timer screen |

## Shared

- The app opens on the timer list, also after it was closed while a timer was
  started.
- Nothing makes a sound. DEFERRED: sound for timers — `doc/deferred.md`.

## Launcher

The app's icon on the phone, labelled with the app name.

### Wording

- app name: "Exercise Inc"

## Timer list

The page the app opens on. The title bar shows the app name and New. Below it,
one row per timer showing its name, in the order the timers were created,
newest last. At the bottom, Start and Delete, which act on the selected timer.
It keeps the platform's default look.

- Tapping a row selects it; one timer is selected at a time. Start and Delete
  are disabled while none is.
- New opens New timer; Start opens the timer screen with the selected timer.
- Delete asks for confirmation. Confirming removes the timer; cancelling keeps
  it.
- After Save on New timer, the new timer is in the list and selected.
- Without timers, the list shows the empty-list text.

### Wording

- title bar: the app name, "New"
- buttons: "Start", "Delete"
- empty-list text: "No timers yet."
- delete confirmation: title "Delete timer?", the timer's name as message,
  buttons "Delete" and "Cancel"

## New timer

A field per setting, the name the values make, Cancel and Save. Both buttons
return to the timer list. It keeps the platform's default look.

- Fields use the numeric keypad. The start delay may be left empty for no
  delay; the other fields start empty.
- The name line shows the name while all values are valid.
- Save is disabled while a value is out of its range or the settings match an
  existing timer. A field out of range shows its allowed range below it; a
  match shows the duplicate text.
- While the keyboard is open it covers Cancel and Save; closing it (its ✓ or
  back) shows them. DEFERRED: keeping them above the keyboard —
  `doc/deferred.md`.

### Wording

- title: "New timer"
- fields: "Start delay (s)", "Work (s)", "Rest (s)", "Repeats"
- start delay hint: "empty: no delay; otherwise 5 or more"
- name line: "Name: {name}"
- buttons: "Cancel", "Save"
- allowed range: "{min} to {max}"; for the start delay "empty, or {min} to
  {max}"
- duplicate text: "This timer already exists."

## Timer screen

It fills the whole screen: no title bar, and the phone's status and navigation
bars are hidden. It shows the started timer as running, paused or done. End and
the end of the Done state return to the timer list.

- **Running**: the phase as a phase label (`doc/style.md` › Phase label), the
  countdown (› Countdown), the repeat line, Pause and End. The background has
  the phase's colour (› Phase colours) and flashes during its last seconds
  (› Last seconds).
- The countdown shows the time left in the phase as `m:ss`, rounded up to the
  whole second: a 30 s work starts at `0:30` and ends as `0:01` runs out.
- The repeat line shows the current repeat and the number of repeats. It is
  hidden during the start delay.
- **Paused**: the paused label above the phase, the countdown held, the repeat
  line, Resume, Restart and End, on the paused background (› Paused).
- **Done**: the done text over the whole screen, on the done background
  (› Done). A tap anywhere returns to the timer list.
- The screen stays on while the timer is running.
- The phone's back gesture pauses a running timer, and ends a paused or done
  one.

### Wording

- phases: "Start delay", "Work", "Rest"
- repeat line: "repeat {current} / {repeats}"
- buttons: "Pause", "End", "Resume", "Restart"
- paused label: "Paused"
- done text: "Done"
