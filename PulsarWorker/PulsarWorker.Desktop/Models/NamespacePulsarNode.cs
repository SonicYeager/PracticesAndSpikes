using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData.Binding;
using PulsarWorker.Client;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches Topics for given namespace name.
/// </summary>
public class NamespacePulsarNode : ReactiveObject, IPulsarNode
{
    private readonly IPulsarClient _pulsarClient;
    private readonly string _tenant;
    public NamespacePulsarNode(string name, string tenant, IPulsarClient pulsarClient)
    {
        Name = name;
        _pulsarClient = pulsarClient;
        _tenant = tenant;
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
        var topics = await _pulsarClient.GetTopics(_tenant, Name);
        if (topics != null)
            foreach (var topic in topics)
            {
                SubNodes.Add(new TopicPulsarNode(topic));
            }

        _loaded = true;
    }
}