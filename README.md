# Exercise Inc

An Android app built with .NET MAUI. For now it holds interval timers; what it
is for is in `doc/product.md`.

## Running it

Prepare the machine first: `doc/setup/index.md`.

```sh
dotnet build                 # build the app for Android
dotnet test                  # run the unit tests
dotnet run scripts/trace.cs  # each section of domain, app and style with the tests naming it, and the gaps
dotnet run scripts/trace.cs --changed  # only the sections changed since main
scripts/run-android.sh       # build, install on the running emulator, open the app
scripts/run-android.sh phone # Release build, install on the phone over USB, open it
scripts/check-style-preview.sh # on the emulator: the style preview shows every token and pattern
```

What each check proves: `doc/architecture/index.md` › Verification.

## Development affordances

- Start the emulator: `doc/setup/android.md` › Create an Android emulator.
- Screenshot: `adb -e exec-out screencap -p > screen.png` of the emulator,
  `adb -d …` of the phone.
- Drive the app from the shell: `. scripts/ui.sh` on the emulator, or
  `UI_DEVICE=-d . scripts/ui.sh` on the phone, then `tap`, `field`, `shot`,
  `texts` (usage at the top of the file).
- Style preview, Debug builds only: timer list › ⋮ › Style preview. One
  section per pattern of `doc/style.md`, drawn with the real styles and
  tokens, then every other colour token; its Run button runs a short timer in
  the timer screen's colours.

## Where truth lives

| question | answer lives in |
|---|---|
| intent, scope, product-wide decisions | `doc/product.md` |
| vocabulary, domain rules, what is kept | `doc/domain.md` |
| surfaces, interaction and wording | `doc/app.md` |
| how it looks and sounds | `doc/style.md` |
| stack, structure, technical rules, verification; what code must respect of each technology | `doc/architecture/` |
| preparing the machine and the phone; manual steps, by technology | `doc/setup/` |
| rejected and deferred options | `doc/deferred.md` |
| supplied material (input, not authority) | `doc/discovery/` |
| open decisions | `git grep -n PROVISIONAL -- ':!.claude'` |
