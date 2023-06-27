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

        //this.WhenAnyValue(x => x.Nodes.When(n => n.)) //TODO abuse this for interaction?
        //    .Subscribe(x =>
        //    {
        //        if(x > 1) throw new InvalidOperationException("Cannot have more than one root node!");
        //    });
    }

    public async Task LoadAsync()
    {
        await _treeModel.GetPulsarNodeTree(Nodes);
    }

    public ObservableCollection<PulsarNodeViewModel> Nodes { get; init; } = new();

}