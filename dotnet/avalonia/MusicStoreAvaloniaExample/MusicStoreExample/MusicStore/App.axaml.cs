using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.ViewModels;
using MusicStore.Views;
using System;

namespace MusicStore;
public class App : Application
{
    public IServiceProvider ServiceProvider { get; set; } = null!;
    public IServiceCollection ServiceCollection { get; } = new ServiceCollection();
    public override void Initialize()
    {
        ServiceCollection.AddTransient<AlbumViewModel>();
        ServiceCollection.AddTransient<MusicStoreViewModel>();
        ServiceCollection.AddTransient<MainWindowViewModel>();
        ServiceCollection.AddTransient<AlbumView>();
        ServiceCollection.AddTransient<MainWindow>(d => new MainWindow()
        {
            DataContext = d.GetRequiredService<MainWindowViewModel>(),
        });
        ServiceCollection.AddTransient<MusicStoreView>();
        ServiceCollection.AddTransient<MusicStoreWindow>();

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