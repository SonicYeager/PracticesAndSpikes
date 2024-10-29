using System.Collections.ObjectModel;
using ReactiveUI;

namespace Cbam.ViewModels;

public class TreeViewItemViewModel : ViewModelBase
{
    private string _header;
    public string Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }

    private ObservableCollection<TreeViewItemViewModel> _children = [];
    public ObservableCollection<TreeViewItemViewModel> Children
    {
        get => _children;
        set => this.RaiseAndSetIfChanged(ref _children, value);
    }
}