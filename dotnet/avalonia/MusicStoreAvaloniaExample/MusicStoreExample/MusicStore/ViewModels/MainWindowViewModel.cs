using MusicStore.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;

namespace MusicStore.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private bool _collectionEmpty;
    
    public ICommand BuyMusicCommand { get; }
    public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }
    public ObservableCollection<AlbumViewModel> Albums { get; } = new();
    
    public MainWindowViewModel()
    {
        BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new MusicStoreViewModel();

            var result = await ShowDialog!.Handle(store);
            
            if (result != null)
            {
                Albums.Add(result);
                
                await result.SaveToDiskAsync();
            }
        });
            
        ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

        this.WhenAnyValue(x => x.Albums.Count)
            .Subscribe(x => CollectionEmpty = x == 0);
        
        RxApp.MainThreadScheduler.Schedule(LoadAlbums);
    }

    public bool CollectionEmpty
    {
        get => _collectionEmpty;
        set => this.RaiseAndSetIfChanged(ref _collectionEmpty, value);
    }
    
    private async void LoadAlbums()
    {
        var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));

        foreach (var album in albums)
        {
            Albums.Add(album);
        }

        foreach (var album in Albums.ToList())
        {
            await album.LoadCover();
        }
    }
}