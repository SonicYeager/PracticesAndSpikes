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

    public ICommand OpenSettings { get; }
    
    public ICommand TogglePane { get; }

    private Control _content = new TextBox { Text = "Here follows some content soon!" };

    public Control Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    //public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

    //public ObservableCollection<AlbumViewModel> Albums { get; } = new();

    public MainWindowViewModel()
    {
        TogglePane = ReactiveCommand.Create( () =>
        {
            PaneState = !PaneState;
        });
        OpenSettings = ReactiveCommand.Create( () =>
        {
            Content = new TextBox{ Text = "Settings page will follow soon!" };
        });

        //ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

        //this.WhenAnyValue(x => x.Albums.Count)
        //    .Subscribe(x => CollectionEmpty = x == 0);

        //RxApp.MainThreadScheduler.Schedule(LoadAlbums);
    }
}