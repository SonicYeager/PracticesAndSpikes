using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using DynamicData.Binding;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches all available Namespaces.
/// </summary>
public class RootPulsarNode : ReactiveObject, IPulsarNode
{

    public IObservableCollection<IPulsarNode> SubNodes { get; init; } = new ObservableCollectionExtended<IPulsarNode>()
    {
        new NamespacePulsarNode("Placeholder")
    };

    public string Name { get; init; } = "Namespaces";

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
            SubNodes.Clear();
            SubNodes.Add(new NamespacePulsarNode("critical"));
            Task.Delay(Random.Shared.Next(0, 100));
            SubNodes.Add(new NamespacePulsarNode("info"));
            Task.Delay(Random.Shared.Next(0, 100));
            SubNodes.Add(new NamespacePulsarNode("news"));
        });
    }
}