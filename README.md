# StabToRestart

A simple BeatSaber mod that allows you to restart a level by committing seppuku.

## How to Install

### Prerequisites

This mod currently requires:
- BSIPA
- SiraUtil
- BeatSaberMarkupLanguage

These requirements can be most easily installed through [ModAssistant](https://github.com/Assistant/ModAssistant/releases/), which is built and maintained by the [BSMG](https://bsmg.wiki/).

### Through the Pre-Compiled .dll

Simply copy the contents of the .zip file in the most recent release in the releases section into the root of your BeatSaber game folder.

### From Source

Import the project into Visual Studio (I believe only VS 2019 or 2022 is supported), and then create a file named `StabToRestart.csproj.user` in the project directory, using the following format

```xml
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <BeatSaberDir>C:\Path\To\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If that file is created correctly, references to dependencies will be automatically resolved

## Possible/Known Issues

- I have only tested this on my own headset (a Quest 2), and have been told the system I used to access buttons presses may not work with SteamVR. Note that the button condition may not work for non-Oculus users.
- When the button condition is not selected, the dropdown menu for the button choice will disappear, while the text will remain.

### Contribute

Any contributions to the source are welcome! Please note that I am new to BeatSaber modding, so it may take awhile for me to verify any pull requests.