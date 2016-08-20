**Build Status**

| Windows                 |  Linux/Mac              | Code Coverage         | NuGet                 |   
| ------------------------|-------------------------|-----------------------|-----------------------|
| [![Build status][1]][2] | [![Build Status][3]][4] | [![codecov][5]][6]    | [![NuGet][7]][8]      |

[1]: https://ci.appveyor.com/api/projects/status/7oo0gyq4fv0jvsjn/branch/master?svg=true
[2]: https://ci.appveyor.com/project/robotdotnet/networktables/branch/master
[3]: https://travis-ci.org/robotdotnet/NetworkTables.svg?branch=master
[4]: https://travis-ci.org/robotdotnet/NetworkTables
[5]: https://codecov.io/gh/robotdotnet/NetworkTables/branch/master/graph/badge.svg
[6]: https://codecov.io/gh/robotdotnet/NetworkTables
[7]: https://img.shields.io/nuget/v/FRC.NetworkTables.svg
[8]: https://www.nuget.org/packages/FRC.NetworkTables

NetworkTables is a DotNet implementation of the NetworkTables protocol commonly used in FRC. Currently implements v3 of the NetworkTables spec.

This repository contains two seperate release projects. The first is NetworkTables, which is a complete port of the ntcore library from C++ to DotNet. This library is recommended for any clients that you wish to create, as the dependancies are very low, and supported by most platforms.
The second project is NetworkTables.Core. This is a wrapper around the official ntcore library. This means that the networking code has been tested more by the community, and is recommended for running on an FRC robot as the server. 



Supported Platforms - NetworkTables
-----------------------------------
* All systems that support the frameworks listed below
* .NET 4.5.1 or higher
* .NET Standard 1.3 or higher:
  * System.Net.NameResolution
  * System.ComponentModel.EventBasedAsync
Xamarin is supported, however see below for issue and workaround

Supported Platforms - NetworkTables.Core
----------------------------------------
* .NET 4.5.1 or higher
* .NET Standard 1.5
* Since this uses a native library, only the platforms listed below are supported
* Windows x86 and amd64
* Linux x86 and amd64
* Mac OS x86 and x86-64
* RoboRio (Soft Float Arm v7)


