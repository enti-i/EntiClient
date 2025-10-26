using System;
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

        var nextTheme = vm.CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        ThemeManager.ApplyTheme(nextTheme);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (DataContext is MainViewModel vm)
        {
            vm.Dispose();
        }
    }
}
