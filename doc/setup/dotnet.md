# Setup: .NET

## Install the .NET SDK in the home directory

- who:      the user, or the agent with the user's go
- why:      authority; it changes the user's shell profile
- action:   no root needed:
            `curl -sSL -o /tmp/dotnet-install.sh https://dot.net/v1/dotnet-install.sh`,
            then `bash /tmp/dotnet-install.sh --channel 10.0
            --install-dir ~/.dotnet`. Append to `~/.bashrc`:
            `export DOTNET_ROOT="$HOME/.dotnet"` and
            `export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"`
- when:     once per machine
- expected: a new shell finds `dotnet`
- verify:   in a new shell, `dotnet --version` prints `10.0.x`
- status:   run 2026-10-04: .NET SDK 10.0.401
