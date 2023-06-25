using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Desktop.ViewModels;
using PulsarWorker.Desktop.Views;
using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using PulsarWorker.Client;
using PulsarWorker.Data.AutoMapper;
using PulsarWorker.Database.Extensions;
using PulsarWorker.Desktop.Models;
using PulsarWorker.Desktop.Services;
using PulsarApi = PulsarWorker.Desktop.Views.PulsarApi;

namespace PulsarWorker.Desktop;

public sealed class App : Application
{
    private IServiceProvider ServiceProvider { get; set; } = null!;
    private IServiceCollection ServiceCollection { get; } = new ServiceCollection();

    private IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .Build();

    public override void Initialize()
    {
        var connectionString = Configuration.GetConnectionString("PulsarWorker");
        ServiceCollection.AddDatabase(connectionString ?? throw new InvalidOperationException("ConnectionString cannot be null!"));
        ServiceCollection.AddAutoMapper(typeof(AutoMapperConfig));
        ServiceCollection.AddTransient<SettingsModel>();
        ServiceCollection.AddTransient<HttpClient>(static d => new()
        {
            BaseAddress = new("http://localhost:8080"),
        });
        ServiceCollection.AddTransient<IPulsarClient, PulsarClient>();
        ServiceCollection.AddTransient<PulsarService>();
        ServiceCollection.AddTransient<MainWindowViewModel>();
        ServiceCollection.AddTransient<PulsarApiViewModel>();
        ServiceCollection.AddTransient<SettingsViewModel>();

        ServiceCollection.AddTransient<Settings>(static d => new()
        {
            DataContext = d.GetRequiredService<SettingsViewModel>(),
        });
        ServiceCollection.AddTransient<PulsarApi>(static d => new()
        {
            DataContext = d.GetRequiredService<PulsarApiViewModel>(),
        });
        ServiceCollection.AddSingleton<MainWindow>(static d => new()
        {
            DataContext = d.GetRequiredService<MainWindowViewModel>(),
        });

        ServiceProvider = ServiceCollection.BuildServiceProvider();

        AvaloniaXamlLoader.Load(this);
        DataTemplates.Add(new ViewLocator(ServiceProvider));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

        base.OnFrameworkInitializationCompleted();
    }
}