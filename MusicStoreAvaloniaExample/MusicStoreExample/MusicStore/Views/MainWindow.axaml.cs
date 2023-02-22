using Avalonia.ReactiveUI;
using MusicStore.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace MusicStore.Views;
public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
    }
        
    private async Task DoShowDialogAsync(InteractionContext<MusicStoreViewModel, AlbumViewModel?> interaction)
    {
        var dialog = new MusicStoreWindow
        {
            DataContext = interaction.Input,
        };

        var result = await dialog.ShowDialog<AlbumViewModel?>(this);
        interaction.SetOutput(result);
    }
}