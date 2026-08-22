[![License: GNU AGPL](https://img.shields.io/badge/License-GNU--AGPL-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![OpenTK](https://img.shields.io/badge/OpenTK-4.9.4-green)](https://opentk.net/)
[![Windows](https://img.shields.io/badge/Windows-10%2B-0078D6)](https://www.microsoft.com/windows)

## **Unminal Engine — Freedom to Will, Freedom to Act**

* The engine **doesn't dictate rules**; it hands you the tools to **bring your vision** to life. From a **flexible console system** to **rendering control** — there are no restrictions here, only possibilities.
* Help us keep this spirit alive: **star the repo** and **watch Unminal evolve!**
* A simple, open-source 3D game engine built from scratch in C# using OpenTK.

### 🗺️ Features
1. Import objects with textures (UV system) \*
2. Update Text Renderer \*\*
3. Update Graphics <br>
3.1 Shape Refactoring: reduction of creation in scripts \*\*\* <br>
3.2 Light Refactoring: creating a separate object \*\*\*\* <br>
4. Implement AABB collision (at beginning only with built-in shapes) \*\*\*\*\* 
5. Create water object (visually, without collision) \*\*\*
6. Particles \*\*

> __Difficulty__ <br>
> __\* Simple | \*\* Medium | \*\*\* Hard | \*\*\*\* So Hard | \*\*\*\*\* Hardcore__

*(Note: This project is in early development. Features like Lua scripting, ECS, and Vulkan support are planned for future versions.)*

## Tech Stack

*   **Language:** C# (.NET 10.0)
*   **Graphics API:** OpenGL 3.3 (via OpenTK 4.9.4)
*   **IDE:** Visual Studio Code / Visual Studio 2022

## Installation & Build

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later.
*   Graphics drivers supporting OpenGL 3.3+.

### Steps
1.  Clone the repository:
    ```bash
    git clone https://github.com/dovintc-off/Unminal-Engine.git
    cd Unminal-Engine
    ```

2.  Restore dependencies (optional, `dotnet run` does this automatically):
    ```bash
    dotnet restore
    ```

3.  Build and Run:
    ```bash
    dotnet run
    ```

## How You Can Help

1.  **Report Bugs:** If you find a crash or unexpected behavior, please open an [Issue](https://github.com/dovintc-off/Unminal-Engine/issues).
2.  **Suggest Features:** Have an idea for a new rendering feature or UI improvement? Let’s discuss it in Discussions or Issues.
3.  **Pull Requests:** Feel free to fork the repo, fix bugs, or add features, and submit a PR. Please follow the coding style of the project.

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## Author
**Dov1ntc -** [GitHub Profile](https://github.com/dovintc-off)

---
*If you like this project, consider giving it a ⭐ on GitHub!*
