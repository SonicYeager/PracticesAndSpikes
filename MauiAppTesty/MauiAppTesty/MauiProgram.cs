using MauiAppTesty.Options;
using MauiAppTesty.ViewModels;
using MauiAppTesty.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiAppTesty;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureEssentials()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .AddPlatformPreferences()
            .Build();

        builder.Configuration.AddConfiguration(config);

        builder.Services.AddLogging(o =>
        {
            o.AddConsole();
        });

        builder.Services.AddOptions<Settings>()
            .Bind(builder.Configuration.GetSection(nameof(Settings)));

        builder.Services
            .AddScoped<HttpClient>()
            .AddTransient<RandomFactsApiPageViewModel>()
            .AddTransient<SettingsPageViewModel>()
            .AddTransient<RandomFactsApiPage>()
            .AddTransient<SettingsPage>()
            .AddTransient<FlyoutFooter>()
            .AddTransient<FlyoutHeader>();

        return builder.Build();
    }
}
