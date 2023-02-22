using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
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
        ServiceCollection.AddTransient<MainWindow>();
        ServiceCollection.AddTransient<MusicStoreView>();
        ServiceCollection.AddTransient<MusicStoreWindow>();

        ServiceProvider = ServiceCollection.BuildServiceProvider();
        
        AvaloniaXamlLoader.Load(this);
        DataTemplates.Add(new ViewLocator());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}