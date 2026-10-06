# Deferred and rejected options

| date | option | status | reason |
|---|---|---|---|
| 2026-10-04 | Build on Windows (.NET SDK on Windows, Rider or Visual Studio) | rejected | The build runs in WSL, where the agent can build, install and inspect the app itself. A Windows build would need a second SDK kept on the same version and the repository on the Windows filesystem. |
| 2026-10-04 | .NET 11 | deferred | Release candidate only; general availability is due November 2026. Revisit after it ships, as a retarget from .NET 10. |
| 2026-10-06 | The look: launcher icon, splash screen, colours beyond the timer phases | deferred | The timer list is the first real screen; the user keeps the platform defaults for now, so the timer work stays about timers. Revisit when the user asks. |
| 2026-10-06 | Sound for timers: a tone at each phase change, ticks in the last seconds | deferred | The user chose a silent timer for now; colour and flashing carry it. Revisit when the user asks. |
| 2026-10-06 | Editing a timer's settings | deferred | The first timer work covers creating and deleting only. Revisit when the user asks. |
| 2026-10-06 | EF Core with SQLite for persistence | rejected | A heavier dependency, a larger app and a slower start, for structure the data does not need yet. sqlite-net-pcl was chosen (`doc/architecture.md`). Revisit if the relational data outgrows it. |
| 2026-10-06 | A JSON file or Android preferences for persistence | rejected | Not built for records; the plan's later data would force the timers to move to another store. |
| 2026-10-06 | Keep New timer's Cancel and Save above the keyboard | deferred | Android 15+ draws the app under the keyboard; neither the soft-input resize mode nor MAUI's keyboard safe area moved the buttons. Closing the keyboard costs one tap. Revisit if it bothers in use. |
