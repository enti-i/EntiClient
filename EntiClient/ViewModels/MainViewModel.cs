using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EntiClient.Models;

namespace EntiClient.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private AppTheme _currentTheme = AppTheme.Dark;
    private bool _isOffline;
    private DateTime _lastSyncTime = DateTime.Now;

    public ObservableCollection<LauncherPreset> Presets { get; } = new()
    {
        new LauncherPreset
        {
            Name = "PvP Velocity",
            Description = "Optimized for competitive play with low latency tweaks.",
            Icon = "⚔",
            Accent = "#FF646B"
        },
        new LauncherPreset
        {
            Name = "Survival Realm",
            Description = "Balanced performance and shaders for immersive survival.",
            Icon = "🌲",
            Accent = "#6BCB77"
        },
        new LauncherPreset
        {
            Name = "Modded Frontier",
            Description = "Forge 1.20 modpack with auto-updating profile management.",
            Icon = "🧪",
            Accent = "#9B6BFF"
        }
    };

    public ObservableCollection<ModItem> FeaturedMods { get; } = new()
    {
        new ModItem { Name = "Sodium", Version = "0.5.8", Status = "Installed" },
        new ModItem { Name = "Iris Shaders", Version = "1.6.4", Status = "Update available" },
        new ModItem { Name = "Replay Mod", Version = "2.6.0", Status = "Not installed" }
    };

    public ObservableCollection<string> OfflineCapabilities { get; } = new()
    {
        "Cached Mojang authentication tokens",
        "Stored player skins & cosmetics",
        "Most recent resource pack bundle",
        "Last synced friends & realm list"
    };

    public string PlayerName { get; } = "EntiPlayer";
    public string Rank { get; } = "Legend";
    public string LastLogin { get; } = DateTime.Now.AddHours(-3).ToString("g");

    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsOffline
    {
        get => _isOffline;
        set
        {
            if (_isOffline != value)
            {
                _isOffline = value;
                if (!value)
                {
                    _lastSyncTime = DateTime.Now;
                    OnPropertyChanged(nameof(LastSync));
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionStatus));
                OnPropertyChanged(nameof(ConnectionTagline));
                OnPropertyChanged(nameof(LaunchButtonText));
                OnPropertyChanged(nameof(HeroPrompt));
                OnPropertyChanged(nameof(CanManageAccounts));
                OnPropertyChanged(nameof(CanCheckUpdates));
                OnPropertyChanged(nameof(OfflineHeadline));
                OnPropertyChanged(nameof(OfflineDescription));
            }
        }
    }

    public string ConnectionStatus => IsOffline ? "Offline mode" : "Connected";

    public string ConnectionTagline => IsOffline
        ? "Using cached profiles and stored resources."
        : "All services responsive and in sync.";

    public string LaunchButtonText => IsOffline ? "Launch offline" : "Launch";

    public bool CanManageAccounts => !IsOffline;

    public bool CanCheckUpdates => !IsOffline;

    public string OfflineHeadline => IsOffline ? "Offline play is ready." : "Prepare your next offline session.";

    public string OfflineDescription => IsOffline
        ? "You'll continue with the last synced configuration until you're back online."
        : "Download updates now so everything is ready when the network drops.";

    public string LastSync => _lastSyncTime.ToString("f");

    public string HeroPrompt => IsOffline
        ? "Offline session staged with the most recent sync."
        : "Ready to jump into your next adventure?";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
