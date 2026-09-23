# How to Contribute

## Local Setup

### Unity-only (most contributors)
Use this if you are only editing C# code, assets, samples, or tests.
1. Ensure you have [Unity 2022.3.23f1](https://unity.com/releases/editor/whats-new/2022.3.23) installed.
2. Fork the repository and clone it to your local drive.

### JS bridge (only if you change the PlayroomKit JS plugin)
Node.js is only required to rebuild the PlayroomKit JS plugin. The build step installs the upstream JS SDK and generates the `.jslib`/`.jspre` files used by Unity.
1. Install [NodeJS](https://nodejs.org/en) on your system.
2. Build the JS bridge:
   ```shell
   cd Tools/playroomkit-js
   npm install
   ```
   This installs the upstream SDK and builds the plugin into `Packages/com.playroomkit.sdk/Runtime/Plugins/Playroom` (the `.jslib` and `.jspre` files).
3. Re-run `npm install` only when `Tools/playroomkit-js/src/index.js` changes.

## Resources
- User-facing Documentation: https://docs.joinplayroom.com/usage/unity
- Join our developer chatroom: https://discord.gg/uDHxeRYhRe
- Email our founder: [tabish@joinplayroom.com](mailto:tabish@joinplayroom.com)

Read more about the PlayroomKit Unity integration and the design behind it.

- [Official announcement blog](https://docs.joinplayroom.com/blog/unityweb)
- [Deep dive into PlayroomKit Unity](https://www.linkedin.com/pulse/building-unity-plugin-javascript-grayhatpk-gynfc/?trackingId=kbv0oZVNT6aLh2TjQ%2FhuVw%3D%3D)

## Types of Tasks
### Upstream Sync - Feature
The Playroom's JavaScript SDK (hereafter referred to as the "JavaScript SDK") is the foundation of this Unity package. For detailed information, visit [this changelog](https://docs.joinplayroom.com/changelog).

To implement a new feature available in the upstream SDK:
1. Create `Internal` functions in the `index.js` file.
2. Develop `C#` bindings for these functions in the `Headers.cs` file.
3. Define methods within the `IInterop` interface and implement them in the `PlayroomKitInterop` class. These are necessary for running tests.
4. Begin implementing methods for the three modes/services: **LocalPlayroomService**, **BrowserPlayroomService**, and **BuildPlayroomService** (all extended from the `IPlayroomBase` interface).
5. Implement these functions across all services based on their specific requirements.
6. The unified function is already handled, so you only need to create a method in `Playroomkit.cs` that calls `CheckPlayRoomInitialized()` and delegates to the implemented methods in each service.

### Upstream Sync - Fix
The process for implementing fixes from the upstream SDK mirrors other feature implementations, typically done within the `index.js` file.

### Architecture
This package follows this folder structure (UPM layout):
```
Packages/com.playroomkit.sdk
├── Assets/
├── Editor/
├── Runtime/
├── Samples~/
├── Tests/
├── package.json
├── Playroom.asmdef
└── PlayroomKit.cs
```
The package also includes a custom WebGL template for Discord activities located in `Packages/com.playroomkit.sdk/Assets/WebGLTemplates`. This can be uploaded to project using the provided menu.

#### Modules
Playroomkit comes with many [modules](https://docs.joinplayroom.com/components) which help with speeding up development. Unity SDK builds on top of that and adds its own modules such as MockMode.
The folder structure is something like this:
```
Packages/com.playroomkit.sdk/Runtime/modules
├── Helpers/
├── Interfaces/
├── MockMode/
├── Options/
├── Player/
└── RPC/
├── Headers
├── PlayroomBuildService
└── PlayroomkitDevManager
```

- **Helpers**: Includes utilities such as CallbackManager, CommandManager, and a Helpers class used for serializing and deserializing data.
- **Interfaces**: Contains all of the base interfaces from which other classes inherit.
- **MockMode**: Holds files for editor-only modes, including Browser and Local mock modes.
- **Options**: Contains classes for settings and options related to different modules, such as InitOptions.
- **Player**: Contains classes related to Player features and functionality.
- **RPC**: Contains classes for implementing Remote Procedure Calls (RPC) related features.
- **Headers.cs**: Contains method declarations which are implemented in `src/index.src`
- **PlayroomBuildService.cs**: Build mode of playroomkit which only runs in the compiled game.
- **PlayroomkitDevManager.cs**: Manager script for choosing between local and browser mockmode, this is used in the `PlayroomMockManager` prefab.

## Tests
Tests are located in `Packages/com.playroomkit.sdk/Tests/Editor` and are currently editor-only. Install Unity Test Runner to execute tests via the `Window/General/Test Runner` menu.

## Samples
Samples are distributed via the package and must be imported into your project to run them.
1. Open `Window > Package Manager`.
2. Select `PlayroomKit` in the list.
3. In the `Samples` section, click `Import` on the sample you want to run.
4. Open the imported sample scene from your project `Assets/` folder.

## Editor Menu Options
Menu options are available to help setup the Unity Editor:
1. `PlayroomKit > Dev > Apply Playroom Mock Mode to Scene` adds the mock mode prefab to active scene
2. `PlayroomKit > WebGL > ...` allow setting up editor support for WebGL/discord builds

### Syncing sample changes
If the package sample content changes, re-import the sample from the same Package Manager screen (`Samples` section > `Reimport`).

Do not edit the imported sample directly in `Assets/` if you want changes to persist upstream. The imported copy is not merged back into the package. If you must tweak something in an imported sample, also apply the same changes to the source sample under `Packages/com.playroomkit.sdk/Samples~` (or manually replace the updated files in the package sample).

