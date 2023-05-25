using System.Windows.Input;
using Avalonia.Controls;
using PulsarWorker.Desktop.Views;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly PulsarApi _pulsarApiView;

    private bool _paneOpen = false;

    public bool PaneState
    {
        get => _paneOpen;
        set => this.RaiseAndSetIfChanged(ref _paneOpen, value);
    }

    public ICommand ShowSettings { get; }

    public ICommand ShowApi { get; }

    public ICommand TogglePane { get; }

    private Control _content = new TextBlock { Text = "Here follows some content soon!" };

    public Control Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    //public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

    public MainWindowViewModel(PulsarApi pulsarApiView)
    {
        _pulsarApiView = pulsarApiView;

        TogglePane = ReactiveCommand.Create(() => { PaneState = !PaneState; });
        ShowSettings = ReactiveCommand.Create(() =>
        {
            Content = new TextBlock { Text = "Settings page will follow soon!" };
            //TODO add ability to set host address and in future to configure auth
        });
        ShowApi = ReactiveCommand.Create(() => { Content = _pulsarApiView; });

        //ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

        //RxApp.MainThreadScheduler.Schedule(LoadAlbums);
    }
}