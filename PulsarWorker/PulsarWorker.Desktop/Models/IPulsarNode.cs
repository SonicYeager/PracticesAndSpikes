using System.Threading.Tasks;
using DynamicData.Binding;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

public interface IPulsarNode
{
    public IObservableCollection<IPulsarNode> SubNodes { get; init; }

    public string Name { get; init; }
    
    public bool IsExpanded { get; set; }
}

public class EmptyPulsarNode : IPulsarNode
{
    public IObservableCollection<IPulsarNode> SubNodes { get; init; } = new ObservableCollectionExtended<IPulsarNode>();
    public string Name { get; init; } = "PLACEHOLDER";
    public bool IsExpanded { get; set; } = false;
}