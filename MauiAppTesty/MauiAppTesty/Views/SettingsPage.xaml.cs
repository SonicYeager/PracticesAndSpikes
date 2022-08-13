using MauiAppTesty.ViewModels;

namespace MauiAppTesty.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPageViewModel _viewModel;
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        _viewModel.Themes = new List<string> { nameof(AppTheme.Dark), nameof(AppTheme.Light) };
        _viewModel.Theme = Preferences.Get("settings/theme", null) ?? Application.Current.RequestedTheme.ToString();
        _viewModel.SaveChanges = new Command(() => SaveChanges());

        BindingContext = _viewModel;
    }

    private void SaveChanges()
    {
        switch (_viewModel.Theme)
        {
            case nameof(AppTheme.Dark):
                Application.Current.UserAppTheme = AppTheme.Dark;
                Preferences.Set("settings/theme", nameof(AppTheme.Dark));
                break;
            case nameof(AppTheme.Light):
                Application.Current.UserAppTheme = AppTheme.Light;
                Preferences.Set("settings/theme", nameof(AppTheme.Light));
                break;
            default:
                break;
        }
    }
}