using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace EntiClient;

public static class ThemeManager
{
    private static readonly string ThemePreferenceDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "EntiClient");

    private static readonly string ThemePreferenceFile = Path.Combine(ThemePreferenceDirectory, "theme.pref");

    public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public static AppTheme ActiveTheme { get; private set; } = AppTheme.Dark;

    public static void ApplyTheme(AppTheme theme, bool persist = true)
    {
        var app = Application.Current;
        if (app == null)
        {
            return;
        }

        if (ActiveTheme == theme && ThemeAlreadyApplied(app, theme))
        {
            if (persist)
            {
                PersistTheme(theme);
            }

            return;
        }

        var dictionaries = app.Resources.MergedDictionaries;
        var existingTheme = dictionaries.FirstOrDefault(d =>
        {
            var source = d.Source?.OriginalString ?? string.Empty;
            return source.EndsWith("DarkTheme.xaml", StringComparison.OrdinalIgnoreCase) ||
                   source.EndsWith("LightTheme.xaml", StringComparison.OrdinalIgnoreCase);
        });

        if (existingTheme != null)
        {
            dictionaries.Remove(existingTheme);
        }

        var themePath = theme switch
        {
            AppTheme.Light => new Uri("Themes/LightTheme.xaml", UriKind.Relative),
            _ => new Uri("Themes/DarkTheme.xaml", UriKind.Relative)
        };

        dictionaries.Add(new ResourceDictionary { Source = themePath });

        var previousTheme = ActiveTheme;
        ActiveTheme = theme;

        if (persist)
        {
            PersistTheme(theme);
        }

        if (previousTheme != theme)
        {
            OnThemeChanged(app, theme);
        }
    }

    public static void LoadPersistedTheme()
    {
        try
        {
            if (!File.Exists(ThemePreferenceFile))
            {
                return;
            }

            var value = File.ReadAllText(ThemePreferenceFile).Trim();
            if (Enum.TryParse<AppTheme>(value, out var theme))
            {
                ApplyTheme(theme, persist: false);
            }
        }
        catch (IOException)
        {
            // Ignore broken preference storage and continue with defaults.
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore storage issues caused by restricted environments.
        }
    }

    private static void PersistTheme(AppTheme theme)
    {
        try
        {
            Directory.CreateDirectory(ThemePreferenceDirectory);
            File.WriteAllText(ThemePreferenceFile, theme.ToString());
        }
        catch (IOException)
        {
            // Ignore persistence failures - the runtime theme already changed.
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore persistence failures - the runtime theme already changed.
        }
    }

    private static bool ThemeAlreadyApplied(Application app, AppTheme theme)
    {
        var dictionaries = app.Resources.MergedDictionaries;
        return dictionaries.Any(d =>
        {
            var source = d.Source?.OriginalString ?? string.Empty;
            return theme switch
            {
                AppTheme.Light => source.EndsWith("LightTheme.xaml", StringComparison.OrdinalIgnoreCase),
                _ => source.EndsWith("DarkTheme.xaml", StringComparison.OrdinalIgnoreCase)
            };
        });
    }

    private static void OnThemeChanged(Application app, AppTheme theme)
    {
        ThemeChanged?.Invoke(app, new ThemeChangedEventArgs(theme));
    }
}

public enum AppTheme
{
    Dark,
    Light
}

public sealed class ThemeChangedEventArgs : EventArgs
{
    public ThemeChangedEventArgs(AppTheme theme)
    {
        Theme = theme;
    }

    public AppTheme Theme { get; }
}
