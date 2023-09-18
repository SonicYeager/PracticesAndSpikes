namespace MauiAppTesty;

public sealed partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
        switch (Preferences.Get("settings/theme", null))
        {
            case nameof(AppTheme.Dark):
                Current.UserAppTheme = AppTheme.Dark;
                UserAppTheme = AppTheme.Dark;
                break;
            case nameof(AppTheme.Light):
                Current.UserAppTheme = AppTheme.Light;
                UserAppTheme = AppTheme.Light;
                break;
            default:
                break;
        }
    }
}