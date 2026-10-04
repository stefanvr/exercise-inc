# Deferred and rejected options

| date | option | status | reason |
|---|---|---|---|
| 2026-10-04 | Build on Windows (.NET SDK on Windows, Rider or Visual Studio) | rejected | The build runs in WSL, where the agent can build, install and inspect the app itself. A Windows build would need a second SDK kept on the same version and the repository on the Windows filesystem. |
| 2026-10-04 | .NET 11 | deferred | Release candidate only; general availability is due November 2026. Revisit after it ships, as a retarget from .NET 10. |
