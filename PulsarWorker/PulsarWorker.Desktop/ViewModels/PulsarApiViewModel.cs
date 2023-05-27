using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DynamicData.Binding;
using PulsarWorker.Desktop.Models;
using PulsarWorker.Desktop.Services;

namespace PulsarWorker.Desktop.ViewModels;

public class PulsarApiViewModel : ViewModelBase
{
    private PulsarService _service;

    public PulsarApiViewModel(PulsarService service)
    {
        _service = service;

        //this.WhenAnyValue(x => x.Nodes.When(n => n.)) //TODO abuse this for interaction?
        //    .Subscribe(x =>
        //    {
        //        if(x > 1) throw new InvalidOperationException("Cannot have more than one root node!");
        //    });
    }

    public async Task LoadAsync()
    {
        await _service.GetPulsarNodeTree(Nodes);
    }

    public ObservableCollection<PulsarNode> Nodes { get; init; } = new();

}