using Application = Microsoft.Maui.Controls.Application;

namespace Practice.Maui;

public sealed partial class App : Microsoft.Maui.Controls.Application
{
    public App(AppShell appShell)
    {
        InitializeComponent();

        MainPage = appShell;
    }
}