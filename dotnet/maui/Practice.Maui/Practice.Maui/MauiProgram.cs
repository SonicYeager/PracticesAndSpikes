﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Practice.Maui.Models;
using Practice.Maui.Services;
using Practice.Maui.ViewModels;
using Practice.Maui.Views;

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
        builder.Services.AddSingleton<FuelStopPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<OverviewViewModel>();
        builder.Services.AddTransient<FuelStopEntryViewModel>();
        builder.Services.AddTransient<FuelBookSheetModel>();
        builder.Services.AddTransient<FuelBookRowModel>();
        builder.Services.AddTransient<SheetServiceWrapper>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}