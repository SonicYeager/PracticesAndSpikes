using System.Collections.ObjectModel;
using ReactiveUI;

namespace Cbam.ViewModels;

public sealed class QReportViewModel : ViewModelBase
{
    private string _header = string.Empty;
    public required string Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }

    private ObservableCollection<QReportViewModel> _children = [];
    public required ObservableCollection<QReportViewModel> Children
    {
        get => _children;
        set => this.RaiseAndSetIfChanged(ref _children, value);
    }

    private bool _isExpanded = true; // Set default to true
    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public required ViewModelBase? Details { get; set; }
}