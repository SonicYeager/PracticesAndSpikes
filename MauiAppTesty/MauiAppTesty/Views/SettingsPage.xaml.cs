using MauiAppTesty.Options;
using MauiAppTesty.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace MauiAppTesty.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPageViewModel _viewModel;
    private readonly IOptions<Settings> _setting;
    public SettingsPage(IOptions<Settings> setting, SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        _setting = setting;
        _viewModel = viewModel;

        _viewModel.Theme = _setting.Value.Theme;
        _viewModel.Themes = new List<string> { nameof(AppTheme.Dark), nameof(AppTheme.Light) };
        _viewModel.SaveChanges = new Command(() => SaveChanges());

        BindingContext = _viewModel;
    }

    private async Task SaveChanges()
    {
        switch (_viewModel.Theme)
        {
            case nameof(AppTheme.Dark):
                Application.Current.UserAppTheme = AppTheme.Dark;
                _setting.Value.Theme = nameof(AppTheme.Dark);
                break;
            case nameof(AppTheme.Light):
                Application.Current.UserAppTheme = AppTheme.Light;
                _setting.Value.Theme = nameof(AppTheme.Light);
                break;
            default:
                break;
        }
    }
}