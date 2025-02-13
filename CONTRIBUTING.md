# How to contribute

## Local setup
Setting up the project locally is similar to any other unity project:
- Make sure you have [Unity `2022.3.23f1`](https://unity.com/releases/editor/whats-new/2022.3.23)
- Make sure you have [NodeJS](https://nodejs.org/en) installed. 
- Fork the repository and clone it.
- Now to build the plugin you can run the following commands in a terminal of your choice, in the project directory:
```shell
cd Assets/Playroomkit
npm install
```
- The above commands should install the `upstream sdk` and its dependencies. It also creates a Playroom folder inside `Assets/Plugins` which should contain 2 files: `JSLIB` and a `JSPRE` file.  
- Your setup is now complete. you'll need to run `npm install` whenever you make changes in `Playroomkit/src/index.js`.

## Types of tasks

### Upstream sync - Feature
_Upstream here means the Playroom's [JavaScript SDK ](https://docs.joinplayroom.com/setup) which is the foundation of this Unity package._

--- 
Implementing a new feature which is availabe in the upstream usually requires the following steps:
- Create `Internal` functions in the `index.js` file.
- Create `C#` bindings for the same functions in the `Headers.cs` file. 
- Declare methods inside `IInterop` interface and implement that method in the `PlayroomKitInterop` class. These are required for running tests.
- Now we can start with the actual implementations for the three modes / services (**`LocalPlayroomService`, `BrowserPlayroomService` and `BuildPlayroomService`**). These are extended from `IPlayroomBase` interface. Create a method declaration here.
- Now, we can simply implement the function in all of the services one by one based on the requirements of the modes / service.
- At the end we need to expose one unified function which can call the correct method based on environment and selected mode, this is already handled. All you need to do now is to create a function in `Playroomkit.cs` which should call `CheckPlayRoomInitialized()` and then call the function you just implemented in the three different modes / services for example: `__playroomservice.YourNewMethod()`


### Upstream sync - Fix
The flow of implementing a fix from upstream SDK is similar, usually the changes are done in the `index.js` file in most cases.

### Fix
The flow of fixing something should be similar be similar to the one described above, this also depends on other parts and modules which are described below in [Architecture](#architecture).

### Feature
The Playroom Unity SDK provides some additional unity specific features such as the local and browser mode, mock mode manager and playroom console. When implementing a feature it may require  external dependencies, which are placed in `Playroomkit\Dependencies` folder. 

### CI / CD
// TODO

### Test
Tests are in the `Playroomkit/Tests` folder, the tests are currently editor only. To run the tests make sure you have `Unity Test Runner` installed. You can run the tests from `Window/General/Test Runner`.

## Architecture
The folder structure of the package is like this:
```
PlayroomKit
├── dependencies/
├── Editor/
├── Examples/
├── modules/
├── node_modules/
├── Prefabs/
├── src/
├── Tests/
├── package.json    
├── package-lock.json
├── Playroom.asmdef 
├── PlayroomKit.cs  
└── vite.config.js 
```
The package also comes with a custom WebGL template for discord activities which is in `Assets/WebGLTemplates` folder.

// TODO explain the modules

### JSLib
The JSLIB file is generated after running the `npm install` command. This acts as a bridge between calling the upstream methods from Unity side using `C#`.

### Limitations
The Upstream SDK is made for web technologies and focuses on makeing games from the web. Currently there is no way of building the game for native platforms. 