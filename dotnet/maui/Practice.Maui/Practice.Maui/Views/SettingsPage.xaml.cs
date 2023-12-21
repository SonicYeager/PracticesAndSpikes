using Practice.Maui.ViewModels;

namespace Practice.Maui.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        BindingContext = settingsViewModel;
        InitializeComponent();
    }
}