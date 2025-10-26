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

    public static AppTheme ActiveTheme { get; private set; } = AppTheme.Dark;

    public static void ApplyTheme(AppTheme theme, bool persist = true)
    {
        var app = Application.Current;
        if (app == null)
        {
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
        ActiveTheme = theme;

        if (persist)
        {
            PersistTheme(theme);
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
            // Ignore persistence failures – the runtime theme already changed.
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore persistence failures – the runtime theme already changed.
        }
    }
}

public enum AppTheme
{
    Dark,
    Light
}
