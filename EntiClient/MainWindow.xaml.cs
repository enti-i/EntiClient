using System.Windows;
using EntiClient.ViewModels;

namespace EntiClient;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnToggleTheme(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        vm.CurrentTheme = vm.CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        ThemeManager.ApplyTheme(vm.CurrentTheme);
    }
}
