using Practice.Maui.Services;
using Practice.Maui.Views;

namespace Practice.Maui;

public sealed partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(FuelStopPage), typeof(FuelStopPage));
    }
}