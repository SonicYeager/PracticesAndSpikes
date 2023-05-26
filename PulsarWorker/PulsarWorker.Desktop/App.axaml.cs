using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Desktop.ViewModels;
using PulsarWorker.Desktop.Views;
using System;
using System.Net.Http;
using PulsarWorker.Client;
using PulsarWorker.Desktop.Models;
using PulsarApi = PulsarWorker.Desktop.Views.PulsarApi;

namespace PulsarWorker.Desktop;

public class App : Application
{
    private IServiceProvider ServiceProvider { get; set; } = null!;
    private IServiceCollection ServiceCollection { get; } = new ServiceCollection();

    public override void Initialize()
    {
        ServiceCollection.AddTransient<HttpClient>(d => new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:8080")
        });
        ServiceCollection.AddTransient<IPulsarClient, PulsarClient>();
        ServiceCollection.AddTransient<IPulsarNode, RootPulsarNode>();
        ServiceCollection.AddSingleton<MainWindowViewModel>();
        ServiceCollection.AddSingleton<PulsarApiViewModel>();
        ServiceCollection.AddSingleton<PulsarApi>(d => new PulsarApi()
        {
            DataContext = d.GetRequiredService<PulsarApiViewModel>()
        });
        ServiceCollection.AddSingleton<MainWindow>(d => new MainWindow
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
        {
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}