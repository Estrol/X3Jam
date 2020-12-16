# X3-JAM, A new O2-JAM 1.8 Server Emulation
Currently the project on early progress.

There two entry point in this project solution:
- `Console` Command-Style based server application for cross platform.
- `WPF` GUI based server application for Windows Only.

## Project Goals
- Multiplayer support.
- Player shop support.
- Website support.

## Requirements
- .NET 5
- System.Data.SQLite

## Compile
- Open solution using Visual Studio 2019
- Select `Estrol.X3Jam.Console` or `Estrol.X3Jam.WPF`
- Press build (and it will automaticly restore the package)

## Supported clients
- Modified 1.8 client (ripped Tcp Obfuscation)
- 1.8 client with DLLInjection

I need know How I can de-obfuscate vanilla 1.8 client packet to able support it without modification or using DLL Injection.

## License
This project licensed under [GPL-V3](/LICENSE.txt)