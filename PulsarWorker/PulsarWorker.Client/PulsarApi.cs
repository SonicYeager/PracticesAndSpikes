using System.Net.Http.Json;

namespace PulsarWorker.Client;

public class PulsarApi : IPulsarApi
{
    private readonly HttpClient _client;

    public PulsarApi(HttpClient client)
    {
        _client = client;
        _client.BaseAddress ??= new Uri("http://localhost:8080/");
    }

    public async Task<IEnumerable<string>?> GetClusters()
    {
        return await _client.GetFromJsonAsync<IEnumerable<string>>(new Uri("/admin/v2/clusters"));
    }

    public Task<IEnumerable<string>?> GetNamespaces(string tenant)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<string>?> GetTopics(string tenant, string pulsarNamespace)
    {
        return await _client.GetFromJsonAsync<IEnumerable<string>>(new Uri("/persistent/:tenant/:namespace"));
    }
    
    public async Task<HttpResponseMessage> DeleteTopic(string tenant, string pulsarNamespace, string topic)
    {
        return await _client.DeleteAsync(new Uri("/persistent/:tenant/:namespace/:topic"));
    }
    
}