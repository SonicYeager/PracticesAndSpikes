using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DynamicData.Binding;
using PulsarWorker.Client;
using PulsarWorker.Desktop.Models;
using ReactiveUI;

namespace PulsarWorker.Desktop.Services;

public sealed class PulsarService
{
    private readonly IPulsarClient _pulsarClient;

    public PulsarService(IPulsarClient pulsarClient)
    {
        _pulsarClient = pulsarClient;
    }

    public async Task GetPulsarNodeTree(ObservableCollection<PulsarNode> collection)
    {
        var mainNode = await GetClusterNode("standalone", n => collection.Remove(n));
        collection.Add(mainNode);
    }

    private async Task<PulsarNode> GetClusterNode(string cluster, Action<PulsarNode> removeSelf)
    {
        return await Load(
            cluster,
            removeSelf,
            async _ => await _pulsarClient.GetTenants(),
            async (node, rmSlf) => await GetTenantNode(node, rmSlf));
    }

    private async Task<PulsarNode> GetTenantNode(string tenant, Action<PulsarNode> removeSelf)
    {
        return await Load(
            tenant,
            removeSelf,
            async parent => await _pulsarClient.GetNamespaces(parent),
            async (node, rmSlf) => await GetNameSpaceNode(tenant, node, rmSlf));
    }

    private async Task<PulsarNode> GetNameSpaceNode(string tenant, string nmspc, Action<PulsarNode> removeSelf)
    {
        return await Load(
            nmspc,
            removeSelf,
            async parent => await _pulsarClient.GetTopics(tenant, parent), static async (node, rmSlf) => await GetTopicNode(node, rmSlf));
    }

    private static async Task<PulsarNode> GetTopicNode(string nmspc, Action<PulsarNode> removeSelf)
    {
        return await Load(
            nmspc,
            removeSelf, static _ => Task.FromResult<IEnumerable<string>?>(null), static (_, _) => Task.FromResult<PulsarNode>(default!));
    }

    private static async Task<PulsarNode> Load(string parentNode, Action<PulsarNode> removeSelf,
        Func<string, Task<IEnumerable<string>?>> loadNodes,
        Func<string, Action<PulsarNode>, Task<PulsarNode>> createNode)
    {
        var collection = new ObservableCollection<PulsarNode>();
        var nodes = await loadNodes(parentNode);
        if (nodes != null)
            foreach (var node in nodes)
            {
                collection.Add(await createNode(node, n => collection.Remove(n)));
            }

        var tenantNode = new PulsarNode(collection, parentNode, ReactiveCommand.Create(removeSelf),
            ReactiveCommand.Create(static () => { }));
        return tenantNode;
    }
}