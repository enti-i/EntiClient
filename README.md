# EntiClient for Windows

EntiClient is a Windows desktop launcher concept for managing Minecraft installations, curated modpacks, and client presets. The project ships as a .NET WPF application that provides a polished, aurora-inspired UI with navigation, stats, and quick actions for common launcher workflows.

## Getting started

1. Install the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or newer with Windows desktop workload enabled.
2. Clone this repository and open a Developer PowerShell or Command Prompt.
3. Restore dependencies and build the application:
   ```powershell
   dotnet build EntiClient.sln
   ```
4. Run the client from the command line or using Visual Studio:
   ```powershell
   dotnet run --project EntiClient/EntiClient.csproj
   ```

## Features

- Dashboard with player profile, quick actions, and animated stats cards.
- Preset manager to highlight PvP, Survival, and Modded launcher profiles.
- Integrated mod manager preview with install/uninstall and update actions.
- Theme toggle between light and dark palettes tailored for desktop UI.
- Offline mode toggle with cached resource overview and sync reminders.
- Responsive layout with gradient backdrops that scales for 1080p and ultrawide displays.

## Roadmap ideas

- Hook into actual Minecraft installation directories and version manifests.
- Implement real authentication against Microsoft accounts.
- Integrate with mod repositories (Modrinth, CurseForge) and auto-update.
- Add news feed, community chat, and screenshot gallery panes.

## License

This project is provided under the MIT license. See [LICENSE](LICENSE) for details.
