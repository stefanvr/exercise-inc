# Exercise Inc

An Android app built with .NET MAUI. For now it holds interval timers; what it
is for is in `doc/product.md`.

## Running it

Prepare the machine first: `doc/setup.md`.

```sh
dotnet build                 # build the app for Android
scripts/run-android.sh       # build, install on the running emulator, open the app
```

There are no automated tests yet (`doc/architecture.md` › Verification).

## Development affordances

- Start the emulator: `doc/setup.md` › Manual steps › Emulator.
- Screenshot of the emulator: `adb exec-out screencap -p > screen.png`.

## Where truth lives

| question | answer lives in |
|---|---|
| intent, scope, product-wide decisions | `doc/product.md` |
| vocabulary, domain rules, what is kept | `doc/domain.md` |
| surfaces, interaction and wording | `doc/app.md` |
| how it looks and sounds | `doc/style.md` |
| stack, structure, technical rules, verification | `doc/architecture.md` |
| preparing the machine; manual steps | `doc/setup.md` |
| rejected and deferred options | `doc/deferred.md` |
| supplied material (input, not authority) | `doc/discovery/` |
| open decisions | `git grep -n PROVISIONAL -- ':!.claude'` |
