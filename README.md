# Age of Mythology Retold Mod Manager

A Tool to manage Mods for Age of Mythology Retold

## Description

This tool allow easier control of active mod for your Age of Mythology Retold installation.
Features include...

- Changing currently active mods directly.
- Changing multiple active mods with one click, by activating a 'Playset'.
- Creating 'Playsets' from...
  - Current mods status, i.e from your `myth-mod-status.json`.
  - Another selected Playset, i.e the current Playset being edited, allowing easy duplication of Playsets.
  - ([WIP](https://github.com/andr9528/AoMR-ModManager/issues/6)) Clipboard, to easily share a Playset with others.
- ([WIP](https://github.com/andr9528/AoMR-ModManager/issues/8)) Change priority of mods in your Playsets.
- ([WIP](https://github.com/andr9528/AoMR-ModManager/issues/9)) Reloading of manual changes to managed files.
- ([WIP](https://github.com/andr9528/AoMR-ModManager/issues/5)) Filter Mods shown in your Playset(s).
- Deleting of Playset files from the tool.
- Renaming of Playset files from the tool.
- Allows to only include relevant mods in Playsets, to improve readability. ([BUGGED](https://github.com/andr9528/AoMR-ModManager/issues/12))
- Creating default Playsets, with either all mods on or off, as that are some Playsets i commonly expect users to want.
- Updates all Playsets on startup with updated data from `myth-mod-status.json`, incase of e.g a mod having been updated.

### How it Works

The tool does its job, by making copies of the `myth-mod-status.json` file, from that point onward referencing them as 'Playsets'. When a 'Playset' is activated, it will overwrite the content of the `myth-mod-status.json` file, with the content of the Playset.

## Installation

To install this tool, download the file from [releases](https://github.com/andr9528/AoMR-ModManager/releases) that fits your operating system.
For windows you would do the following.

1. Download the `.exe` file from the release page.
2. Place it in the `C:\Users\[Your Username]\Games\Age of Mythology Retold\[A bunch of Numbers]\mods\` folder of your 'Age of Mythology Retold' installation.

Make sure there is a `myth-mod-status.json` in that folder.
Age of Mythology Retold should have generated it, if you have done anything with mods before, otherwise find a mod you like and download it, that should at the very least generate it.

I Can highly suggest the '5x Relics' Mod.

## Feedback

Do you have any feedback you wish to share with me or others?
In that case, create a post in the [Feedback discussion category](https://github.com/andr9528/AoMR-ModManager/discussions/categories/feedback).

## For Developers

### Localization

Do you want the tool to support a new language?
Follow the steps below to create it yourself, and in the end create a pull request back to my repository, for it to become part of the tool for good.

1. Fork this repository.
2. Clone it locally so you can begin to make adjustments.
3. Clone e.g 'Resources.en.resx' file under 'ModManager/Strings/', placing the new file in the same folder.
4. Rename the language part of new file, e.g 'en', to the Shorthand Iso Name of the target language, e.g 'da' for danish.
   1. Set 1 codes found on [Wikipedia](https://en.wikipedia.org/wiki/List_of_ISO_639_language_codes) should be the correct ones.
5. Update the translations inside the cloned file.
   1. I can highly recommend using Visual Studio while doing this, but any text editor should suffice.
6. Add the new translation name to the array of Cultures in the [appsettings](ModManager/appsettings.json) file.
7. Commit your changes back to Github.
   1. I try to follow [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0) for the commit message. Would be happy if you did too.
8. Create a pull request from your forked repository back to my repository.
9. Wait for me to Approve and merge it.
   1. If i do not react within a week, then fell free to try and reach out to me on Discord (WolfDK#2337).
10. Wait for me to realease a new version of the tool OR build a version yourself from your fork.
11. Done

### Frameworks & Nuget Packages

The program of course uses a number of frameworks and nuget packages to achieve its goal. Below a list of used ones, with a short description of its usage, can be seen. Some of those might have other nuget packages they use, which i will not list.

#### Frameworks

##### Uno Platform

The frontend framework used to develop one code that can run on Windows, Mac and Linux.

##### NUnit

The testing framework of choice, used to (eventually) test smaller different pieces of the program.

#### Nuget Packages

##### FluentAssertions

Nuget package to make tests easier to read, as a human.

##### Newtonsoft.Json

My choice for serializer / deserializer of the json files modified by the program.

##### FastCloner

Nuget package with functionality to copy classes deeply, for making new playsets from the 'myth-mod-status' json file.

##### Serilog.Sinks.File

Nuget package that allow my logging framework of choice, to log to files.
