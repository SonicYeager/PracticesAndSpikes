using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
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

    //public ObservableCollection<AlbumViewModel> Albums { get; } = new();

    public MainWindowViewModel()
    {
        TogglePane = ReactiveCommand.Create(() => { PaneState = !PaneState; });
        ShowSettings = ReactiveCommand.Create(() =>
        {
            Content = new TextBlock { Text = "Settings page will follow soon!" };
            //TODO add ability to set host address and in future to configure auth
        });
        ShowApi = ReactiveCommand.Create(() =>
        {
            Content = new TextBlock { Text = "Api page will follow soon!" };
            //TODO just enable to add/delete a topic or namespace
            //and observe any given topics in any given namespace as a tree view with live view
        });

        //ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

        //this.WhenAnyValue(x => x.Albums.Count)
        //    .Subscribe(x => CollectionEmpty = x == 0);

        //RxApp.MainThreadScheduler.Schedule(LoadAlbums);
    }
}