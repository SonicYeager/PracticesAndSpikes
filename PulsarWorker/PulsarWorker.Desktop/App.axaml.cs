using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Desktop.ViewModels;
using PulsarWorker.Desktop.Views;
using System;
using System.Net.Http;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.Configuration;
using PulsarWorker.Client;
using PulsarWorker.Data.AutoMapper;
using PulsarWorker.Database.Extensions;
using PulsarWorker.Desktop.Models;
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

        private void RegisterServices()
        {
            Services.AddAutoMapper(typeof(AutoMapperConfig));
            Services.AddTransient<SettingsModel>();
            Services.AddTransient(static _ => new HttpClient
            {
                BaseAddress = new("http://localhost:8080"), //TODO enable change of address via settings
            });
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
            Services.AddSingleton<MainWindow>(_ => new()
            {
                DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>(),
            });
            Services.AddSingleton<IManagedNotificationManager, WindowNotificationManager>(
                _ => new(ServiceProvider.GetRequiredService<MainWindow>())
                {
                    Position = NotificationPosition.BottomRight, MaxItems = 5,
                });
        }

        private void BuildServiceProvider()
        {
            ServiceProvider = Services.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            base.OnFrameworkInitializationCompleted();
        }
    }
}