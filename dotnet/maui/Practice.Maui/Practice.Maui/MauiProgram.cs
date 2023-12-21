using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Practice.Maui.Clients;
using Practice.Maui.Models;
using Practice.Maui.ViewModels;
using Practice.Maui.Views;
using Refit;

namespace Practice.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(static fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<OverviewPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<OverviewViewModel>();
        builder.Services.AddTransient<ApodViewModel>();
        builder.Services.AddTransient<ApodModel>();

        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(
                new()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = false,
                }
            ),
        };
        builder.Services.AddRefitClient<INasaApodApi>(refitSettings)
            .ConfigureHttpClient(static c => c.BaseAddress = new("https://api.nasa.gov"));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}