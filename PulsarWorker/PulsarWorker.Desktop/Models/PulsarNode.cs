using System.Collections.ObjectModel;
using DynamicData.Binding;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches all available Namespaces.
/// </summary>
public sealed class PulsarNode
{
    public PulsarNode(ObservableCollection<PulsarNode> subNodes, string name, IReactiveCommand deleteCommand,
        IReactiveCommand addCommand)
    {
        SubNodes = subNodes;
        Name = name;
        DeleteCommand = deleteCommand;
        AddCommand = addCommand;
    }

    public ObservableCollection<PulsarNode> SubNodes { get; init; }
    public string Name { get; init; }
    public IReactiveCommand DeleteCommand { get; init; }
    public IReactiveCommand AddCommand { get; set; }
}