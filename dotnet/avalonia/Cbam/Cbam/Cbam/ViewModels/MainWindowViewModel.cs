using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Cbam.Models;
using ReactiveUI;

namespace Cbam.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> AddFileCommand { get; }

    public Interaction<Unit, Uri?> ShowOpenFileDialog { get; } = new();

    private ObservableCollection<QReportViewModel> _reportTree = new();
    public ObservableCollection<QReportViewModel> ReportTree
    {
        get => _reportTree;
        set => this.RaiseAndSetIfChanged(ref _reportTree, value);
    }

    private bool _loaded;
    public bool Loaded
    {
        get => _loaded;
        set => this.RaiseAndSetIfChanged(ref _loaded, value);
    }

    private QReportViewModel? _selectedItem;
    public QReportViewModel? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public async Task AddFile()
    {
        var result = await ShowOpenFileDialog.Handle(Unit.Default);
        if (!string.IsNullOrEmpty(result?.ToString()))
        {
            Loaded = true;
            ReportTree = LoadReportTree(result.ToString());
        }
    }

    private static ObservableCollection<QReportViewModel> LoadReportTree(string filePath)
    {
        var tree = new ObservableCollection<QReportViewModel>
        {
            QReportReader.Read(filePath),
        };
        return tree;
    }

    public MainWindowViewModel()
    {
        AddFileCommand = ReactiveCommand.CreateFromTask(AddFile);
    }
}