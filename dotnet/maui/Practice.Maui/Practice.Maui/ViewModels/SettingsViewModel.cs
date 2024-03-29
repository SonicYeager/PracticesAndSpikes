using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Practice.Maui.ViewModels;

public sealed class SettingsViewModel
{

    public SettingsViewModel()
    {
        Microsoft.Maui.Controls.Application.Current!.UserAppTheme = Microsoft.Maui.Controls.Application.Current.RequestedTheme;
        Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += ToggleThemeAsync;
        ToggleTheme = new RelayCommand(ToggleThemeAsync);
    }
    public ICommand ToggleTheme { get; }

    private static void ToggleThemeAsync()
    {
        if (Microsoft.Maui.Controls.Application.Current!.UserAppTheme == AppTheme.Dark)
            Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Light;
        else
            Microsoft.Maui.Controls.Application.Current!.UserAppTheme = AppTheme.Dark;
    }

    private static void ToggleThemeAsync(object? s, AppThemeChangedEventArgs eventArgs)
    {
        Microsoft.Maui.Controls.Application.Current!.UserAppTheme = eventArgs.RequestedTheme;
    }
}