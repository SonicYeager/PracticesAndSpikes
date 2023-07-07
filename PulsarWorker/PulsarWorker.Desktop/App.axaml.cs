using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Desktop.ViewModels;
using PulsarWorker.Desktop.Views;
using System;
using System.Net.Http;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;
using Microsoft.Extensions.Configuration;
using PulsarWorker.Client;
using PulsarWorker.Data.AutoMapper;
using PulsarWorker.Database.Extensions;
using PulsarWorker.Desktop.Models;
using PulsarWorker.Desktop.Services;
using PulsarApi = PulsarWorker.Desktop.Views.PulsarApi;

namespace PulsarWorker.Desktop
{
    public sealed class App : Application
    {
        private IServiceProvider ServiceProvider { get; set; } = null!;
        private IServiceCollection Services { get; } = new ServiceCollection();

        private IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        public override void Initialize()
        {
            RegisterDatabase();
            RegisterServices();
            BuildServiceProvider();

            AvaloniaXamlLoader.Load(this);
            DataTemplates.Add(new ViewLocator(ServiceProvider));
        }

        private void RegisterDatabase()
        {
            var connectionString = Configuration.GetConnectionString("PulsarWorker");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ConnectionString cannot be null!");

            Services.AddDatabase(connectionString);
        }

        private new void RegisterServices()
        {
            Services.AddAutoMapper(typeof(AutoMapperConfig));
            Services.AddTransient<SettingsModel>();
            Services.AddTransient<HttpClientFactory>();
            Services.AddTransient<IPulsarClient, PulsarClient>();
            Services.AddTransient<PulsarTreeModel>();
            Services.AddTransient<MainWindowViewModel>();
            Services.AddTransient<PulsarApiViewModel>();
            Services.AddTransient<SettingsViewModel>();
            Services.AddTransient<Settings>(_ => new()
            {
                DataContext = ServiceProvider.GetRequiredService<SettingsViewModel>(),
            });
            Services.AddTransient<PulsarApi>(_ => new()
            {
                DataContext = ServiceProvider.GetRequiredService<PulsarApiViewModel>(),
            });
            Services.AddSingleton<SettingsManager>();
            Services.AddSingleton<UserManager>();
            Services.AddSingleton<MainWindow>(_ => new()
            {
                DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>(),
            });
            Services.AddSingleton<IManagedNotificationManager, WindowNotificationManager>(
                _ => new(ServiceProvider.GetRequiredService<MainWindow>())
                {
                    Position = NotificationPosition.BottomRight, MaxItems = 5,
                });
            Services.AddSingleton<App>(_ => this);
        }

        private void BuildServiceProvider()
        {
            ServiceProvider = Services.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            var settingsManager = ServiceProvider.GetRequiredService<SettingsManager>();
            settingsManager.OnSettingChanged += (key, value) =>
            {
                if (key == "App Theme")
                    RequestedThemeVariant = (value as string) switch
                    {
                        "Default" => ThemeVariant.Default,
                        "Dark" => ThemeVariant.Dark,
                        "Light" => ThemeVariant.Light,
                        _ => throw new ArgumentOutOfRangeException(nameof(value), "Theme value could not be parsed."),
                    };
            };

            base.OnFrameworkInitializationCompleted();
        }
    }
}