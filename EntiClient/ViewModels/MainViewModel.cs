using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EntiClient.Models;
using Microsoft.Win32;

namespace EntiClient.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private AppTheme _currentTheme = AppTheme.Dark;
    private bool _isOffline;
    private DateTime _lastSyncTime = DateTime.Now;
    private string _offlineUsername = "Steve";
    private string? _offlineSkinPath;
    private ImageSource? _offlineSkinPreview;

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

    public MainViewModel()
    {
        BrowseSkinCommand = new RelayCommand(_ => BrowseForSkin(), _ => IsOffline);
        ClearSkinCommand = new RelayCommand(_ => ClearOfflineSkin(), _ => !string.IsNullOrEmpty(_offlineSkinPath) && IsOffline);
    }

    public string PlayerName { get; } = "EntiPlayer";
    public string Rank { get; } = "Legend";
    public string LastLogin { get; } = DateTime.Now.AddHours(-3).ToString("g");

    public ICommand BrowseSkinCommand { get; }

    public ICommand ClearSkinCommand { get; }

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
                OnPropertyChanged(nameof(OfflineProfileSummary));
                CommandManager.InvalidateRequerySuggested();
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
        ? $"Launch using {OfflineUsername} and the resources you staged here."
        : "Download updates now, then tailor your offline persona for the next trip.";

    public string LastSync => _lastSyncTime.ToString("f");

    public string HeroPrompt => IsOffline
        ? "Offline session staged with the most recent sync."
        : "Ready to jump into your next adventure?";

    public string OfflineUsername
    {
        get => _offlineUsername;
        set
        {
            if (_offlineUsername != value)
            {
                _offlineUsername = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OfflineDescription));
                OnPropertyChanged(nameof(OfflineProfileSummary));
            }
        }
    }

    public string OfflineSkinStatus => string.IsNullOrEmpty(_offlineSkinPath)
        ? "Default Alex/Steve skin in use"
        : $"Custom skin: {Path.GetFileName(_offlineSkinPath)}";

    public ImageSource? OfflineSkinPreview
    {
        get => _offlineSkinPreview;
        private set
        {
            if (!Equals(_offlineSkinPreview, value))
            {
                _offlineSkinPreview = value;
                OnPropertyChanged();
            }
        }
    }

    public string OfflineProfileSummary => $"{OfflineUsername} • {(string.IsNullOrEmpty(_offlineSkinPath) ? "Default skin" : Path.GetFileName(_offlineSkinPath))}";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void BrowseForSkin()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Minecraft skin",
            Filter = "Skin files (*.png)|*.png|Image files|*.png;*.jpg;*.jpeg",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog() == true)
        {
            SetOfflineSkin(dialog.FileName);
        }
    }

    private void ClearOfflineSkin()
    {
        SetOfflineSkin(null);
    }

    private void SetOfflineSkin(string? path)
    {
        if (_offlineSkinPath == path)
        {
            return;
        }

        _offlineSkinPath = path;
        UpdateOfflineSkinPreview(path);
        OnPropertyChanged(nameof(OfflineSkinStatus));
        OnPropertyChanged(nameof(OfflineProfileSummary));
        CommandManager.InvalidateRequerySuggested();
    }

    private void UpdateOfflineSkinPreview(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            OfflineSkinPreview = null;
            return;
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            OfflineSkinPreview = bitmap;
        }
        catch
        {
            OfflineSkinPreview = null;
        }
    }
}
