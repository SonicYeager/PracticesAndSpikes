using System;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Desktop.Views;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;

    private bool _paneOpen = false;
    public bool PaneState
    {
        get => _paneOpen;
        set => this.RaiseAndSetIfChanged(ref _paneOpen, value);
    }

    public ICommand ShowSettings { get; }
    public ICommand ShowApi { get; }
    public ICommand TogglePane { get; }

    private Control _content = new TextBlock
    {
        Text = "Here follows some content soon!",
    };

    public Control Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    //public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        TogglePane = ReactiveCommand.Create(() => { PaneState = !PaneState; });
        ShowSettings = ReactiveCommand.Create(async () =>
        {
            var settingsView = _serviceProvider.GetRequiredService<Settings>();
            await (settingsView.DataContext as SettingsViewModel)?.LoadAsync()!;
            Content = settingsView;
        });
        ShowApi = ReactiveCommand.Create(async () =>
        {
            var apiView = _serviceProvider.GetRequiredService<PulsarApi>();
            await (apiView.DataContext as PulsarApiViewModel)?.LoadAsync()!;
            Content = apiView;
        });

        //ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

        //RxApp.MainThreadScheduler.Schedule(LoadAlbums);
    }
}