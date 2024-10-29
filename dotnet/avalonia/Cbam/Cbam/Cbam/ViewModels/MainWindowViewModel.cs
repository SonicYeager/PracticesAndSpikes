using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace Cbam.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> AddFileCommand { get; }

    public Interaction<Unit, Uri?> ShowOpenFileDialog { get; } = new();

    private string? _report;
    public string? Report
    {
        get => _report;
        set => this.RaiseAndSetIfChanged(ref _report, value);
    }

    private ObservableCollection<TreeViewItemViewModel> _reportTree = new();
    public ObservableCollection<TreeViewItemViewModel> ReportTree
    {
        get => _reportTree;
        set => this.RaiseAndSetIfChanged(ref _reportTree, value);
    }

    public async Task AddFile()
    {
        var result = await ShowOpenFileDialog.Handle(Unit.Default);
        if (!string.IsNullOrEmpty(result?.AbsolutePath))
        {
            //TODO actual handling of report
            Report = $"Report loaded from: {result.AbsolutePath}";
            // TODO: Populate the tree structure
            ReportTree = LoadReportTree(result.AbsolutePath);
        }
    }

    private ObservableCollection<TreeViewItemViewModel> LoadReportTree(string filePath)
    {
        // TODO: Implement logic to load the tree structure from the report file
        var tree = new ObservableCollection<TreeViewItemViewModel>
        {
            new()
            {
                Header = "Root",
                Children = new()
                {
                    new()
                    {
                        Header = "Child 1",
                    },
                    new()
                    {
                        Header = "Child 2",
                    },
                },
            },
        };
        return tree;
    }

    public MainWindowViewModel()
    {
        AddFileCommand = ReactiveCommand.CreateFromTask(AddFile);
    }
}