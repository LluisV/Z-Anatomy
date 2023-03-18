# PlayerPrefs Editor for Unity 3D

[![Minimal unity editor version](https://img.shields.io/badge/UnityEditor-2019.4%20or%20later-blue.svg)](https://unity3d.com/de/get-unity/download/archive)
[![CI](https://github.com/Dysman/bgTools-playerPrefsEditor/workflows/CI/badge.svg)](https://github.com/Dysman/bgTools-playerPrefsEditor/actions)&nbsp;&nbsp;
[![Release](https://img.shields.io/github/v/release/Dysman/bgTools-playerPrefsEditor?include_prereleases&label=Release)](https://github.com/Dysman/bgTools-playerPrefsEditor/releases)
[![GitHub package.json version (branch)](https://img.shields.io/github/package-json/v/dysman/bgTools-playerPrefsEditor/upm?label=GitURL-UPM)](https://github.com/Dysman/bgTools-playerPrefsEditor/tree/upm)
[![openupm](https://img.shields.io/npm/v/com.bgtools.playerprefseditor?label=OpenUPM&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.bgtools.playerprefseditor)
[![AssetStore](https://img.shields.io/badge/dynamic/xml?url=http://u3d.as/1RLa&label=UnityAssetStore&query=//*[contains(@class,%20%27product-version%27)]/div[2]&prefix=v)](http://u3d.as/1RLa)

[![Flattr this git repo](https://img.shields.io/badge/_-Flattr-green?logo=flattr&style=flat)](https://flattr.com/@dysman)&nbsp;
[![Buy me a coffee](https://img.shields.io/badge/-Buy%20Me%20A%20Coffee-yellow?logo=BuyMeACoffee&style=flat&logoColor=white)](https://www.buymeacoffee.com/dysman)
[<img align="right" src="https://img.shields.io/discord/431522155814191116?logo=Discord&logoColor=white&style=flat&label=Discord&labelColor=5865F2">](https://discord.gg/8rcPZrD)

Tool extension for the Unity Editor that enables easy access to the player preferences over a simple UI. Allows to view, add, remove and modify entries on the development machine.

![Preference editor window](https://www.bgranzow.de/downloads/PlayerPrefsEditorV1_2_0.png)

## Features

* Add, remove and edit PlayerPrefs
* Intuitive visual editor
* Works with standard Unity PlayerPrefs
* Monitors changes from code
* Supports all editors (Windows, Linux, MacOS)
* Lightweight dockable for full integration in your workflow
* Supports both skins (Personal, Professional)

## Requirements

Unity Version: 2019.4 (LTS) or higher

Editor Version: Windows, MacOS, Linux

## Installation

The plugin provides *manual* and *UPM* installation.


Additionally it's available on the [Unity Asset Store](http://u3d.as/1RLa).

### Manual
Place the PlayerPrefsEditor folder somewhere in your project. It's not relevant where it's located, the plugin will find all of its files by itself.

### Unity Package Manager (UPM)

**Via Git URL**

Through the Unity Plugin Manager it's possible to install the plugin direct from this git repository.
The UPM need a specific structure what will be provided into the *upm* branch.

Use following direct URL for the configuration:
```
https://github.com/Dysman/bgTools-playerPrefsEditor.git#upm
```
See official Unity documentation for more informations: [UI](https://docs.unity3d.com/Manual/upm-ui-giturl.html) or [manifest.json](https://docs.unity3d.com/Manual/upm-git.html)

**Via OpenUPM**

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.bgtools.playerprefseditor
```

## Usage

The entry to open the _PlayerPrefs Editor_ is located in the top menu at Tools/BG Tools/PlayerPrefs Editor. It's a standard dockable window, so place it wherever it helps to be productive.
A more detailed manual can be fund in following locations:
* GitHub (Manual)- [Manual page](Packages/PlayerPrefsEditor/Documentation~/PlayerPrefsEditor.md)
* GitHub (UPM) - Press the _Documentation_ link on the UPM description.
* Unity Asset Store Package - [MANUAL.html](Documentation/MANUAL.html)
