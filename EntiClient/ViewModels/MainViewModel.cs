using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EntiClient.Commands;
using EntiClient.Models;
using Microsoft.Win32;

namespace EntiClient.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    private AppTheme _currentTheme = ThemeManager.ActiveTheme;
    private bool _isOffline;
    private DateTime _lastSyncTime = DateTime.Now;
    private readonly string _onlinePlayerName = "EntiPlayer";
    private string _offlineUsername = "EntiTraveler";
    private ImageSource? _offlineSkinPreview;
    private string? _selectedSkinFileName;

    public ObservableCollection<LauncherPreset> Presets { get; } = new()
    {
        new LauncherPreset
        {
            Name = "PvP Velocity",
            Description = "Optimized for competitive play with low latency tweaks.",
            Icon = "PVP",
            Accent = "#FF646B"
        },
        new LauncherPreset
        {
            Name = "Survival Realm",
            Description = "Balanced performance and shaders for immersive survival.",
            Icon = "SRV",
            Accent = "#6BCB77"
        },
        new LauncherPreset
        {
            Name = "Modded Frontier",
            Description = "Forge 1.20 modpack with auto-updating profile management.",
            Icon = "MOD",
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
        "Personalized offline identity",
        "Most recent resource pack bundle",
        "Last synced friends & realm list"
    };

    public RelayCommand BrowseSkinCommand { get; }
    public RelayCommand ResetSkinCommand { get; }

    public string PlayerName => IsOffline ? OfflineUsername : _onlinePlayerName;
    public string Rank { get; } = "Legend";
    public string LastLogin { get; } = DateTime.Now.AddHours(-3).ToString("g");

    public string OfflineUsername
    {
        get => _offlineUsername;
        set
        {
            if (_offlineUsername != value)
            {
                _offlineUsername = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayerName));
            }
        }
    }

    public ImageSource? OfflineSkinPreview
    {
        get => _offlineSkinPreview;
        private set
        {
            if (!Equals(_offlineSkinPreview, value))
            {
                _offlineSkinPreview = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasOfflineSkin));
                ResetSkinCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasOfflineSkin => OfflineSkinPreview != null;

    public string SelectedSkinName => _selectedSkinFileName ?? "Using cached default skin";

    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThemeToggleLabel));
            }
        }
    }

    public string ThemeToggleLabel => CurrentTheme == AppTheme.Dark ? "Switch to light theme" : "Switch to dark theme";

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
                OnPropertyChanged(nameof(PlayerName));
                BrowseSkinCommand.RaiseCanExecuteChanged();
                ResetSkinCommand.RaiseCanExecuteChanged();
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

    public MainViewModel()
    {
        BrowseSkinCommand = new RelayCommand(BrowseSkin, () => IsOffline);
        ResetSkinCommand = new RelayCommand(ResetSkin, () => IsOffline && HasOfflineSkin);
        ThemeManager.ThemeChanged += OnThemeChanged;
    }

    public void Dispose()
    {
        ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void BrowseSkin()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select an offline skin",
            Filter = "Minecraft skin (*.png)|*.png|Image files|*.png;*.jpg;*.jpeg",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            using var stream = File.OpenRead(dialog.FileName);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();

            OfflineSkinPreview = bitmap;
            _selectedSkinFileName = dialog.SafeFileName;
            OnPropertyChanged(nameof(SelectedSkinName));
        }
        catch
        {
            OfflineSkinPreview = null;
            _selectedSkinFileName = null;
            OnPropertyChanged(nameof(SelectedSkinName));
        }
    }

    private void ResetSkin()
    {
        OfflineSkinPreview = null;
        _selectedSkinFileName = null;
        OnPropertyChanged(nameof(SelectedSkinName));
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (_currentTheme != e.Theme)
        {
            CurrentTheme = e.Theme;
        }
    }
}
