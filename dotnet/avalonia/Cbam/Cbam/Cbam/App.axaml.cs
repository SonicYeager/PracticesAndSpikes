using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Cbam.ViewModels;
using Cbam.ViewModels.QReportDetails;
using Cbam.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Cbam;

public sealed class App : Application
{
    public IServiceProvider ServiceProvider { get; set; } = null!;
    public IServiceCollection ServiceCollection { get; } = new ServiceCollection();

    public override void Initialize()
    {
        ServiceCollection.AddSingleton<MainWindow>(static d => new()
        {
            DataContext = d.GetRequiredService<MainWindowViewModel>(),
        });

        ServiceCollection.AddSingleton<MainWindowViewModel>();
        ServiceCollection.AddSingleton<QReportViewModel>();
        ServiceCollection.AddSingleton<QReportDetailsViewModel>();
        ServiceCollection.AddSingleton<DeclarantDetailsViewModel>();
        ServiceCollection.AddSingleton<ActorAddressDetailsViewModel>();

        ServiceProvider = ServiceCollection.BuildServiceProvider();

        AvaloniaXamlLoader.Load(this);
        DataTemplates.Add(new ViewLocator(ServiceProvider));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

        base.OnFrameworkInitializationCompleted();
    }
}