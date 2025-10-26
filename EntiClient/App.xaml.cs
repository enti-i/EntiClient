using System.Windows;

namespace EntiClient;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ThemeManager.LoadPersistedTheme();
        base.OnStartup(e);
    }
}
