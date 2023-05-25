using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData.Binding;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches Topics for given namespace name.
/// </summary>
public class NamespacePulsarNode : ReactiveObject, IPulsarNode
{
    public NamespacePulsarNode(string name)
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

    private async void LoadAsync()
    {
        await Task.Run(() =>
        {
            SubNodes.Add(new TopicPulsarNode("location"));
            Task.Delay(Random.Shared.Next(0, 100));
            SubNodes.Add(new TopicPulsarNode("store"));
            Task.Delay(Random.Shared.Next(0, 100));
            SubNodes.Add(new TopicPulsarNode("load"));
        });
    }
}