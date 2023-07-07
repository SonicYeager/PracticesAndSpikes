using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PulsarWorker.Client;
using PulsarWorker.Desktop.Services;
using PulsarWorker.Desktop.ViewModels.Components;
using ReactiveUI;

namespace PulsarWorker.Desktop.Models;

public sealed class PulsarTreeModel
{
    private readonly IPulsarClient _pulsarClient;
    private readonly SettingsManager _settingsManager;

    public PulsarTreeModel(IPulsarClient pulsarClient, SettingsManager settingsManager)
    {
        _pulsarClient = pulsarClient;
        _settingsManager = settingsManager;

        _settingsManager.OnSettingChanged += (key, value) =>
        {
            if (key == AvailableSettings.PulsarHostOptionKey && value is string hostAddress && !string.IsNullOrEmpty(hostAddress))
                _pulsarClient.ChangeBaseAddress(new(hostAddress));
        };
    }

    public async Task GetPulsarNodeTree(ObservableCollection<PulsarNodeViewModel> collection)
    {
        var mainNode = await GetClusterNode("standalone", n => collection.Remove(n));
        collection.Add(mainNode);
    }

    private async Task<PulsarNodeViewModel> GetClusterNode(string cluster, Action<PulsarNodeViewModel> removeSelf)
    {
        return await Load(
            cluster,
            removeSelf,
            async _ => await _pulsarClient.GetTenants(),
            async (node, rmSlf) => await GetTenantNode(node, rmSlf));
    }

    private async Task<PulsarNodeViewModel> GetTenantNode(string tenant, Action<PulsarNodeViewModel> removeSelf)
    {
        return await Load(
            tenant,
            removeSelf,
            async parent => await _pulsarClient.GetNamespaces(parent),
            async (node, rmSlf) => await GetNameSpaceNode(tenant, node, rmSlf));
    }

    private async Task<PulsarNodeViewModel> GetNameSpaceNode(string tenant, string nmspc, Action<PulsarNodeViewModel> removeSelf)
    {
        return await Load(
            nmspc,
            removeSelf,
            async parent => await _pulsarClient.GetTopics(tenant, parent), static async (node, rmSlf) => await GetTopicNode(node, rmSlf));
    }

    private static async Task<PulsarNodeViewModel> GetTopicNode(string nmspc, Action<PulsarNodeViewModel> removeSelf)
    {
        return await Load(
            nmspc,
            removeSelf, static _ => Task.FromResult<IEnumerable<string>?>(null),
            static (_, _) => Task.FromResult<PulsarNodeViewModel>(default!));
    }

    private static async Task<PulsarNodeViewModel> Load(string parentNode, Action<PulsarNodeViewModel> removeSelf,
        Func<string, Task<IEnumerable<string>?>> loadNodes,
        Func<string, Action<PulsarNodeViewModel>, Task<PulsarNodeViewModel>> createNode)
    {
        var collection = new ObservableCollection<PulsarNodeViewModel>();
        var nodes = await loadNodes(parentNode);
        if (nodes != null)
            foreach (var node in nodes)
            {
                collection.Add(await createNode(node, n => collection.Remove(n)));
            }

        var tenantNode = new PulsarNodeViewModel(collection, parentNode, ReactiveCommand.Create(removeSelf),
            ReactiveCommand.Create(static () => { }));
        return tenantNode;
    }
}