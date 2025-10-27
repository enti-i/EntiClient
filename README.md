# EntiClient for Windows

EntiClient is a concept launcher for Minecraft built with Windows Presentation Foundation (WPF). The project focuses on presenting a modern, aurora-inspired interface that demonstrates how curated modpacks, offline personas, and launcher utilities could be organised in a single desktop experience.

## Table of contents
- [Features](#features)
- [Screenshots](#screenshots)
- [Project structure](#project-structure)
- [Getting started](#getting-started)
- [Development workflow](#development-workflow)
- [Troubleshooting](#troubleshooting)
- [Roadmap ideas](#roadmap-ideas)
- [License](#license)

## Features
- **Dashboard overview** with player identity, rotating stats, and quick launch actions tailored to online/offline state.
- **Offline persona editor** that lets you pick a username, upload or clear a custom skin, and preview the selected texture before launching offline play.
- **Preset gallery** highlighting PvP, Survival, and Modded profiles to demonstrate how different launch configurations could be showcased.
- **Mod management preview** that surfaces featured mods with install/update status messaging.
- **Theme switching** between light and dark palettes implemented through shared resource dictionaries.

## Screenshots
This repository only contains the source code. Build and run the client locally to explore the interface.

## Project structure
```
EntiClient.sln
└── EntiClient/
    ├── App.xaml, App.xaml.cs      # Application entry point & global resources
    ├── MainWindow.xaml(.cs)       # Primary launcher layout
    ├── Models/                    # UI model classes (e.g. RelayCommand, ModItem)
    ├── ViewModels/                # MVVM view models (MainViewModel drives the UI)
    ├── Themes/                    # Light, dark, and shared resource dictionaries
    └── ThemeManager.cs            # Helper for switching theme resource dictionaries
```

## Getting started
1. Install the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or newer with the **Windows desktop** workload enabled.
2. Clone the repository and open a Developer PowerShell or Command Prompt in the project folder.
3. Restore dependencies and build the solution:
   ```powershell
   dotnet build EntiClient.sln
   ```
4. Launch the client from the command line or via Visual Studio:
   ```powershell
   dotnet run --project EntiClient/EntiClient.csproj
   ```

## Development workflow
- **MVVM pattern**: The `MainViewModel` exposes all launcher state, including offline persona details, preset data, and command bindings. UI commands are implemented with the reusable `RelayCommand` helper under `Models/`.
- **Themes**: `ThemeManager` swaps between the `Themes/LightTheme.xaml` and `Themes/DarkTheme.xaml` resource dictionaries at runtime.
- **Assets**: Placeholder icons and gradients are defined directly in XAML. Replace them with production-ready assets as needed.

## Troubleshooting
- **Duplicate XAML page items**: If MSBuild reports `NETSDK1022` about duplicated `Page` elements, remove manual `<Page>` includes from the project file or set `EnableDefaultPageItems` to `false`. The repository already relies on implicit includes.
- **Missing Windows Desktop SDK**: Ensure the workload was installed by running `dotnet workload install windowsdesktop`.
- **Skin preview not appearing**: Only PNG/JPEG files are supported, and they must remain accessible after selection so the preview bitmap can load.

## Roadmap ideas
- Integrate with actual Minecraft installation manifests and profile management APIs.
- Wire up Microsoft account authentication and cloud synchronisation.
- Connect to Modrinth/CurseForge for live mod discovery and updates.
- Add news, social, or screenshot panels to expand the launcher shell.

## License

This project is provided under the MIT license. See [LICENSE](LICENSE) for details.
