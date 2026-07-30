# All changes in project Unminal

## [0.2.4-prerelease-2] - 2026.07.31
### **Added**
- **2D Primitives:** *Square* (Rectangle), *Circle*, and *Triangle* (26.07)
- **Billboard Class:** A rectangle that *always faces the player's camera* (26.07)
- **New Path Types:** Added `"shader:/"` and `"texture:/"` support in *GetCorrectPath()* (26.07 & 27.07)
- **GameObject Rotation:** Added `Rotate()` method to `GameObject` class (delta time is handled internally) (27.07)
- **Flexible Mesh Attributes:** Mesh class now *supports a variable number of vertex attributes* (27.07)
- **Logging System:** Added `Console.CreateLog` for unified output to both `System` and `Engine` consoles (28.07)
- **Fonts:** Added *Arial* and *Arial Bold* fonts with *corresponding engine path variables* (30.07)
- **Command Registration:** Added `AddCommand` method to *register custom Command objects* under a specific command name (30.07)

### **Changed**
- **Shader Organization:** All shaders are now *located in a single folder* (`Assets/shaders/`) (26.07)
- **Global Matrices:** `Projection`, `View`, and `Ortho` matrices are now *automatically managed by the Engine class* (Engine.View, Engine.Projection, Engine.Ortho). No manual calculation required (26.07)
- **Automatic Matrix Passing:** Engine methods *now automatically use global projection matrices* (custom matrix support will be added later) (26.07)
- **Console Replacement:** Standard `System.Console` has been *replaced by the custom engine Console* class (27.07)
- **Text Rendering:** Increased *font atlas size* and *optimized text display* settings for better quality (30.07)
- **Core Logging:** Replaced all `Console.WriteLine` calls in core files with the *new customized logging system* (30.07)
- **Path Handling:** `Billboard` and `TextRenderer` classes now *automatically resolve file paths internally* (30.07)

### **Fixed**
- **Command Initialization:** Commands are now `initialized once in Program.cs`. Previously, the command structure was created on the first input, causing a freeze/stutter (30.07)
- **Duplicate Logs:** Fixed an issue where duplicate log messages were displayed in the console (30.07)
- **Text Performance:** Multi-colored text is now rendered in a single Draw Call (30.07)

### **For Devs**
- **Build Process:** The *Assets folder is now automatically copied to the build* output directory during compilation (27.07)

## [0.2.4-prerelease-1] - 2026.07.25
### **Added**
- Loaded paths are displayed in the console (not inside the game)
- Simplify the creation of an object

### **Changed**
- Refactored global state management in `Main.cs` using `Engine.GlobalWindowState`
- Removed `FrameUpdateVars` class; scripts now access input/time data directly via `Engine` class
- The console file has been split into `ArgumentProcessor.cs`, `CommandExecutor.cs`, `CommandTokenizer.cs`

### **Fixed**
- Eliminated duplicate resource loading caused by double `_userGame.Load()` call

## [0.2.3] - 2026.07.24
### **Added**
- **Resource Path Resolution System:** Implemented `GetPath` utility class with support for virtual path prefixes (`assets:/`, `data:/`, `font:/`, `obj:/`). Ensures **cross-platform compatibility** and prevents path traversal attacks (..).

- **Commands System Architecture:**
    - Created *modular command execution pipeline* (`CommandParser`, `ExecutedMethods`, `Structure`).
    - Added **verification** for command arguments and support for `different types` in args.
    - Implemented `range()` and `get()` methods for *dynamic data* access via console.

- **Advanced Text Rendering:**
    - Integrated `RichTextSegment` system *for multi-colored* text rendering.
    - Added support for special characters (`;`, `\`, etc.) *and color tags* in console output.
    - Implemented thread-safe glyph storage using `ConcurrentDictionary`. **(Thanks to Mikle Roznecky)**

- **Configuration Management:**
    - Rewrote config loader (ported logic from `SyncraRPC`) with JSON serialization.
    - *Moved configuration files* to dedicated Assets/data/ directory.

- **Camera & Projection:**
    - Added FOV clamping (`Min: 30 degeers`, `Max: 120 degeers`).

- **Project Infrastructure:**
    - Added `CONTRIBUTORS.md` and updated `LICENSE`.
    - Created `feat/server-extension` branch foundation.

### **Changed**
- **Major Project Restructuring:** *Reorganized entire codebase* into clear modules: `core/`, `render/`, `ui/`, `utils/`, `scripts/`.

- **UI Refactoring:**
    - *Separated UI components* into dedicated folders (`InputFieldRender`, `TextRender`).
    - Game *status is now displayed* in the window title bar.

- **Engine Values:** Centralized global state management in `EngineValues` class (`src/core/State.cs`).

### **Fixed**
- **Null Reference Safety:** Resolved all **"possible null reference"** warnings across the solution.
- **Compiler Warnings:** Removed all `#pragma warning disable` directives; fixed `CS8603` and `CS8601` *nullable warnings natively*.
- **Console Stability:** Fixed bug where console would crash on specific inputs; improved text parsing without color tags.
- **Code Parser:** Deleted legacy code parser and implemented custom solution for better reliability.
- **Branch Management:** Cleaned up obsolete branches and merged main into feature branches correctly.

### **Tech Debt / Known Issues**
- **Physics engine is not yet implemented (planned for v0.4 release).**
- **Server extension is in early development stage.**
- **Some UI elements still lack full styling consistency.**


## [0.2.2-alpha] - 2026.05.03

### Added
- Developer Console: Implemented the core `GameConsole` class with history logging and file persistence.
- Console Toggle: Added key binding for the Grave Accent key (`~` / `` ` ``) to open and close the console overlay.
- 2D Primitive System: 
    - Introduced abstract base class `Primitive2D` for handling 2D geometry rendering.
    - Implemented `Square` class as the first concrete primitive, supporting position, scale, rotation, and color customization.

### Fixed
- Resolved various initialization and rendering issues in the graphics pipeline.
- Fixed shader compilation errors related to uniform location binding.

## [0.2.1] - 2026.04.30
### Added
- " | " and " \ " symbol 
- Config File

## [0.2.0] - 2026.04.24
### Added
- Engine architecture: The basic structure of the engine has been implemented, divided into “Core” (Main.cs) and “Game Content” (GameBase.cs / Script.cs).
- Scene System: Added abstract class BaseGame with lifecycle methods Load(), Update() and Draw().
- Modular camera: The camera logic has been moved to a separate class Camera.cs with support for WASD, mouse and zoom control (FOV 30°–110°).
- Global imports: Implemented the GlobalUsings.cs file to automatically include the main OpenTK libraries and engine modules throughout the project.
- Entities (GameObject): The GameObject class was created to encapsulate meshes, shaders and transformations (position, scale, color), which simplifies working with the scene.

### Changed
- Main.cs refactoring: Hard-coded game logic (teapot, skybox) has been completely removed from the main window class. Now Main acts only as a container and render manager.
- Input control: The keyboard and mouse processing logic has been moved inside the Camera class and the Update method of the user's game.
- Code optimization: Removed unnecessary using from files thanks to global imports. Namespace name conflicts have been fixed (the Unminal.Camera namespace has been renamed/managed through aliases).

### Fixed
- Bug with FOV: Fixed the bug of “breaking” the viewing angle when zooming with the mouse wheel (added the correct Clamp range from 30 to 110 degrees).
- Bug CS1612: Fixed problem with changing coordinates of Vector3 structures in the GameObject class (now full vector assignment is used).
- Bug CS0118: Resolved conflict between namespace name and Camera class name.

### Tech Debt / Known Issues
- Light is still transmitted as hardcode to the shader. The plans include creating a Light class and a lighting manager.
- Collisions have not yet been implemented.
- There is no resource loading system with path checking (if the file is not there, the game may crash).

## [0.1.1] - 2026.04.21

### Changed
- Improved readability of the debug menu (in code)

## [TD-0.1.1] - 2026.04.21

### Add
- Dynamic Text Color: Implemented support for changing text color via Vector3 parameter in DrawString.
- Invariant Culture Formatting: Fixed number formatting in debug overlay to always use dots (`.`) instead of commas (`,`) regardless of system locale.
- Created dedicated branch text-development for experimental text features.

### Changed
- Improved readability of the debug menu (in code)

## [0.1.0] - 2026.04.21 (Anniversary!!!)

### Changed
- The menu with additional information opens only when you press f3
- A menu with additional information displays the position and rotation of the camera
- Now each file is in its own namespace (render/Mesh.cs -> namespace Unminal.render)

### Added
- New classes make it easier to create more than one object

## [0.0.7] - 2026.04.21

### Added
- F11 toggles fullscreen/windowed mode.
- Default window state is now windowed.

### Changed
- Moved game logic (input, camera, matrices) to OnUpdateFrame.
- Separated update logic from rendering code.

## [0.0.6] - 2026.04.18

### Fixed
- Fixed lighting visibility issue caused by incorrect render order and uniform binding sequence.

## [0.0.5] - 2026.04.17

### Added
- Text Render (Padding, Resizing, Coordinates)
- [Vertex Shader](Assets/shaders/shader.vert) and [Fragment Shader](Assets/shaders/shader.frag) from text

## [0.0.4] - 2026.04.17

### Added
- Generation Text Atlas
- Text rendering base class. (GlyphData)

## [0.0.3] - 2026.04.17

### Changed
- Renamed project files and directory from **3dGame** to **Unminal**
- Updated root namespace from **BPX** to **Unminal**
- README.md

### Added
- LICENSE
- CHANGELOG.md

### Removed
- Debug Files (debug_atlas_test.png)