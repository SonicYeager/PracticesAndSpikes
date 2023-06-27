using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PulsarWorker.Desktop.Models;
using PulsarWorker.Desktop.ViewModels.Components;

namespace PulsarWorker.Desktop.ViewModels;

public sealed class PulsarApiViewModel : ViewModelBase
{
    private PulsarTreeModel _treeModel;

    public PulsarApiViewModel(PulsarTreeModel treeModel)
    {
        _treeModel = treeModel;
    }

    public async Task LoadAsync()
    {
        await _treeModel.GetPulsarNodeTree(Nodes);
    }

    public ObservableCollection<PulsarNodeViewModel> Nodes { get; init; } = new();

}