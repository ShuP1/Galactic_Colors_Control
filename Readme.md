# Galactic Colors Control

GCC is a cross plateforme C# minimal RTS. 

### Prerequisities

* .NET/[Mono](https://github.com/mono/mono)

For GUI
```
* OpenGL
* [OpenAL](https://www.openal.org/) => :warning: Not included in Windows :warning:
```

### Installing

Download last version for your system

Linux
```
sudo apt-get install libopenal-dev mono-runtime
```

## Running

* Galactic_Colors_Control_Console.exe => Client without GUI
* Galactic_Colors_Control_GUI.exe => Client
* Galactic_Colors_Control_Server.exe => Server (Use --debug for DEBUG MODE or --dev at your risks)

Linux
```
mono <program>.exe --args
```

### Build

[Teamcity server](http://sheychen.ddns.net:8111?guest=1) is self hosted on a raspberry pi.
It's slow and can be off.

| Name  | Status |
|:---|--------|
| Realease | [![Build Status](http://sheychen.ddns.net:8111/app/rest/builds/buildType:GalacticColorsControl_Build/statusIcon)](http://sheychen.ddns.net:8111/viewType.html?buildTypeId=GalacticColorsControl_Build&guest=1) |
| Develop | [![Build Status](http://sheychen.ddns.net:8111/app/rest/builds/buildType:GalacticColorsControl_BuildDevelop/statusIcon)](http://sheychen.ddns.net:8111/viewType.html?buildTypeId=GalacticColorsControl_BuildDevelop&guest=1) |

## Test With

* Windows 10 x64
* XUbuntu 16.04

## Contributing

Get [Monogame](https://github.com/MonoGame/MonoGame) sdk and [MyMonoGame.GUI](https://github.com/sheychen290/MyMonoGame)
As you wish, I am opened to new ideas.

## Author

* **[Sheychen](https://sheychen.shost.ca)** - *Initial work*

See also the list of [contributors](https://github.com/sheychen290/Galactic_Colors_Control/contributors) who participated in this project.

## Using

* .Net/[Mono](https://github.com/mono/mono)
* [Monogame](https://github.com/MonoGame/MonoGame)
* [MyMonoGame.GUI](https://github.com/sheychen290/MyMonoGame)
* [MyCommon](https://github.com/sheychen290/MyCommon)
* [MyConsole](https://github.com/sheychen290/MyConsole)
* [Space Sprites](https://gamedevelopment.tutsplus.com/articles/enjoy-these-totally-free-space-based-shoot-em-up-sprites--gamedev-2368)

## License

This software comes under the terms of the GPLv3+. Previously under MIT license. See the [License.md](License.md) file for the complete text of the license.