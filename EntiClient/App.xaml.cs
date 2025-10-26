using System.Windows;

namespace EntiClient;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.LoadPersistedTheme();
    }
}
