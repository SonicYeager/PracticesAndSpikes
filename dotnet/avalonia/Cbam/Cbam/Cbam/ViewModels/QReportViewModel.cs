using System.Collections.ObjectModel;
using ReactiveUI;

namespace Cbam.ViewModels;

public class QReportViewModel : ViewModelBase
{
    private string _header;
    public string Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }

    private ObservableCollection<QReportViewModel> _children = [];
    public ObservableCollection<QReportViewModel> Children
    {
        get => _children;
        set => this.RaiseAndSetIfChanged(ref _children, value);
    }
}