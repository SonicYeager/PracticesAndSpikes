using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using PulsarWorker.Client;
using PulsarWorker.Desktop.ViewModels;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

/// <summary>
/// Fetches all available Namespaces.
/// </summary>
public class RootPulsarNode : ReactiveObject, IPulsarNode
{
    private readonly IPulsarClient _pulsarClient;

    public RootPulsarNode(IPulsarClient pulsarClient)
    {
        _pulsarClient = pulsarClient;

        Actions.Add(new Action("Create Tenant", ReactiveCommand.Create(async () => await CreateTenant())));
    }

    public IObservableCollection<IPulsarNode> SubNodes { get; init; } = new ObservableCollectionExtended<IPulsarNode>()
    {
        new EmptyPulsarNode()
    };

    public string Name { get; init; } = "Tenants";

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

    public IObservableCollection<IAction> Actions { get; set; } = new ObservableCollectionExtended<IAction>();

    public Interaction<ViewModelBase, IPulsarNode?> Interaction;

    private async Task CreateTenant()
    {
        var res = await Interaction.Handle(new CreateTenantDialogViewModel(_pulsarClient));
        if (res != null)
            SubNodes.Add(res);
    }

    private async void LoadAsync()
    {
        var tenants = await _pulsarClient.GetTenants();
        if (tenants != null)
            foreach (var tenant in tenants)
            {
                SubNodes.Add(new TenantPulsarNode(tenant, _pulsarClient));
            }

        _loaded = true;
    }
}