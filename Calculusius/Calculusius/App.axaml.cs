using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Calculusius.ViewModels;
using Calculusius.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Calculusius;

public class App : Application
{
    public IServiceProvider ServiceProvider { get; set; } = null!;
    public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

    public override void Initialize()
    {
        ServiceCollection.AddTransient<MainWindowViewModel>();
        ServiceCollection.AddTransient<MainWindow>(d => new MainWindow
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