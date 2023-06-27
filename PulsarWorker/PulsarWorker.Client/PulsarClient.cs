using System.Net.Http.Json;

namespace PulsarWorker.Client;

public sealed class PulsarClient : IPulsarClient
{
    private readonly HttpClient _client;

    public PulsarClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress ??= new("http://localhost:8080");
    }

    public async Task<IEnumerable<string>?> GetClusters()
    {
        return await GetResourcesAsync(new Uri("/admin/v2/clusters", UriKind.Relative));
    }

    public async Task<IEnumerable<string>?> GetTenants()
    {
        return await GetResourcesAsync(new Uri("/admin/v2/tenants", UriKind.Relative));
    }

    public async Task<IEnumerable<string>?> GetNamespaces(string tenant)
    {
        return await GetResourcesAsync(new Uri($"/admin/v2/namespaces/{tenant}", UriKind.Relative));
    }

    public async Task<IEnumerable<string>?> GetTopics(string tenant, string pulsarNamespace)
    {
        return await GetResourcesAsync(new Uri($"/persistent/{tenant}/{pulsarNamespace}", UriKind.Relative));
    }

    public async Task<HttpResponseMessage> DeleteTopic(string tenant, string pulsarNamespace, string topic)
    {
        return await _client.DeleteAsync(new Uri($"/persistent/{tenant}/{pulsarNamespace}/{topic}", UriKind.Relative));
    }

    private async Task<IEnumerable<string>?> GetResourcesAsync(Uri uri)
    {
        try
        {
            return await _client.GetFromJsonAsync<IEnumerable<string>>(uri);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}