# Setup: .NET MAUI

## Install MAUI for Android on Linux, with its Android SDK and JDK

- who:      the user, or the agent with the user's go
- why:      authority; it accepts the Android SDK's licences on the user's
            behalf, and changes the user's shell profile
- action:   with the .NET SDK installed (`dotnet.md` › Install the .NET SDK in
            the home directory).
  1. `dotnet workload install maui-android`. Linux has no `maui` workload;
     `maui-android` is MAUI there.
  2. `dotnet build src/ExerciseInc -t:InstallAndroidDependencies
     -f net10.0-android -p:AndroidSdkDirectory=$HOME/Android/Sdk
     -p:JavaSdkDirectory=$HOME/.jdk/microsoft-openjdk
     -p:AcceptAndroidSDKLicenses=True` installs the Linux Android SDK and
     Microsoft OpenJDK, the only JDK .NET for Android supports.
  3. Append `export JAVA_HOME="$HOME/.jdk/microsoft-openjdk"` to `~/.bashrc`
     and remove any other `JAVA_HOME` line. The build finds the SDK by itself
     afterwards, the JDK only through `JAVA_HOME`: without it, it takes
     Ubuntu's OpenJDK and warns about a Java 21 on the `PATH` that is a runtime
     without `jar`.
- when:     once per machine
- expected: the Android build runs on Microsoft OpenJDK
- verify:   in a new shell, `echo $JAVA_HOME` prints the
            `~/.jdk/microsoft-openjdk` path, and `dotnet build` succeeds
- status:   run 2026-10-04: maui-android 10.0.401.1, Microsoft OpenJDK
            17.0.14, platform `android-36`, build-tools 36.0.0
