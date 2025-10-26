using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace EntiClient;

public static class ThemeManager
{
    private const string ThemePreferenceFile = "theme.pref";

    public static void ApplyTheme(AppTheme theme)
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
        PersistTheme(theme);
    }

    public static void LoadPersistedTheme()
    {
        if (!File.Exists(ThemePreferenceFile))
        {
            return;
        }

        var value = File.ReadAllText(ThemePreferenceFile).Trim();
        if (Enum.TryParse<AppTheme>(value, out var theme))
        {
            ApplyTheme(theme);
        }
    }

    private static void PersistTheme(AppTheme theme)
    {
        File.WriteAllText(ThemePreferenceFile, theme.ToString());
    }
}

public enum AppTheme
{
    Dark,
    Light
}
