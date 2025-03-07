# How to Contribute

## Local Setup
Setting up the project locally is similar to any other Unity project:
1. Ensure you have [Unity 2022.3.23f1](https://unity.com/releases/editor/whats-new/2022.3.23) installed.
2. Install [NodeJS](https://nodejs.org/en) on your system.
3. Fork the repository and clone it to your local drive.
4. To build the plugin, run these commands in your terminal:
   ```shell
   cd Assets/Playroomkit
   npm install
   ```
   This command will install the `upstream.sdk` and its dependencies. It also creates a `Playroom` folder inside `Assets/Plugins`, containing two files: a `.JSLIB` file and a `.JSPRE` file.
5. Complete your setup by running `npm install` whenever changes are made to `Playroomkit/src/index.js`.

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
This package follows this folder structure:
```
PlayroomKit
├── dependencies/
├── Editor/
├── Examples/
├── modules/
├── node_modules/
├── Prefabs/
├── src/
└── Tests/
└── package.json    
└── package-lock.json
└── Playroom.asmdef 
└── PlayroomKit.cs  
└── vite.config.js 
```
The package also includes a custom WebGL template for Discord activities located in `Assets/WebGLTemplates`.

#### Modules
Playroomkit comes with many [modules](https://docs.joinplayroom.com/components) which help with speeding up development. Unity SDK builds on top of that and adds its own modules such as MockMode.
The folder structure is something like this:
```
modules
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
Tests are located in the `Playroomkit/Tests` folder and are currently editor-only. Install Unity Test Runner to execute tests via the `Window/General/Test Runner` menu.