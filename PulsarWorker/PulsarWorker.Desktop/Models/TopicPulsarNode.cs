using System.Reactive.Concurrency;
using DynamicData.Binding;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches Topics for given namespace name.
/// </summary>
public class TopicPulsarNode : ReactiveObject, IPulsarNode
{
    public TopicPulsarNode(string name)
    {
        Name = name;
    }

    public IObservableCollection<IPulsarNode> SubNodes { get; init; } = new ObservableCollectionExtended<IPulsarNode>();
    public string Name { get; init; }
    
    private bool _isExpanded = false;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (this.RaiseAndSetIfChanged(ref _isExpanded, value))
            {
                RxApp.MainThreadScheduler.Schedule(LoadAsync);
            }
        }
    }

    public IObservableCollection<IAction> Actions { get; set; } = new ObservableCollectionExtended<IAction>();

    private static void LoadAsync()
    {
        //TODO fill me pls :)
    }
}