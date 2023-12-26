using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Practice.Maui.ViewModels;

public sealed class SettingsViewModel
{
    public ICommand ToggleTheme { get; }

    public SettingsViewModel()
    {
        Application.Current!.UserAppTheme = Application.Current.RequestedTheme;
        ToggleTheme = new RelayCommand(ToggleThemeAsync);
    }

    private static void ToggleThemeAsync()
    {
        if (Application.Current!.UserAppTheme == AppTheme.Dark)
            Application.Current.UserAppTheme = AppTheme.Light;
        else
            Application.Current!.UserAppTheme = AppTheme.Dark;
    }
}