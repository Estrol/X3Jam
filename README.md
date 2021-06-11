# X3-JAM, A re-written O2-JAM Server
Cross-platform and complete re-written O2-JAM 1.8 Server, included multiplayer.

## Current status
The server software may stable or not stable at same time and might ecounter missing tcp opcode handler. \
Current status of the project can be seen [here](/projects/1)

## Downloads
Pre-built binaries is not available right now, but you can compile it.

## Building the server.
### Prerequisites
- .NET 5
- Visual Studio 2019

### Building
Currently only providing build throught Visual Studio Solution.
- Open solution using Visual Studio 2019
- Select `Estrol.X3Jam.Console`
- Press build (and it will automaticly restore the package)

## Supported clients
Any vanilla 1.8 client is supported with Special Launcher (Available in Estrol.X3Jam.Launcher, but it may cause Anti-virus alert.)

## License
This software licensed under [GPL-3.0](/LICENSE.txt) with some MIT Licensed code. 

[tl;dr](https://tldrlegal.com/license/gnu-general-public-license-v3-(gpl-3)) You may copy, distribute and modify the software as long as you track changes/dates in source files. Any modifications to or software including (via compiler) GPL-licensed code must also be made available under the GPL along with build & install instructions.

MIT Licensed code that used in this project:
- [SirusDoma](https://github.com/SirusDoma)'s [O2MusicList](https://github.com/SirusDoma/O2MusicList) (OJN, OJNDecoder, OJNFileFormat, OJNGenre, OJNList, OJNListDecoder)
