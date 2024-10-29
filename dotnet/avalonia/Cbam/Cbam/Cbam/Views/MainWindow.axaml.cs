using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using Cbam.ViewModels;
using ReactiveUI;

namespace Cbam.Views;

public sealed partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(disposables =>
        {
            ViewModel!.ShowOpenFileDialog.RegisterHandler(async interaction =>
            {
                interaction.SetOutput(await OpenFile());
            }).DisposeWith(disposables);
        });
        InitializeComponent();
    }

    private async Task<Uri> OpenFile()
    {
        var selectedFiles = await StorageProvider.OpenFilePickerAsync(new()
        {
            AllowMultiple = false,
            FileTypeFilter =
            [
                new("xml")
                {
                    Patterns =
                    [
                        "*.xml",
                    ],
                },
            ],
        });

        return selectedFiles.Single().Path;
    }
}