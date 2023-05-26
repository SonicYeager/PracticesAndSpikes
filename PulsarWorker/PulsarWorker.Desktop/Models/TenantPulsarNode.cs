using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using PulsarWorker.Client;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches Topics for given namespace name.
/// </summary>
public class TenantPulsarNode : ReactiveObject, IPulsarNode
{
    private readonly IPulsarClient _pulsarClient;
    public TenantPulsarNode(string name, IPulsarClient pulsarClient)
    {
        Name = name;
        _pulsarClient = pulsarClient;
    }

    public IObservableCollection<IPulsarNode> SubNodes { get; init; }
        = new ObservableCollectionExtended<IPulsarNode>()
        {
            new TopicPulsarNode("PLACEHOLDER")
        };
    public string Name { get; init; }
    
    private bool _isExpanded = false;
    private bool _loaded = false;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (this.RaiseAndSetIfChanged(ref _isExpanded, value))
            {
                if (_isExpanded && !_loaded)
                {
                    SubNodes.Clear();
                    RxApp.MainThreadScheduler.Schedule(LoadAsync);
                }
            }
        }
    }

    private async void LoadAsync()
    {
        var namespaces = await _pulsarClient.GetNamespaces(Name);
        if (namespaces != null)
            foreach (var namespc in namespaces)
            {
                SubNodes.Add(new NamespacePulsarNode(namespc, Name, _pulsarClient));
            }

        _loaded = true;
    }
}