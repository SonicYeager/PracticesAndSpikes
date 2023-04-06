using System.Net.Http.Json;
using PulsarWorker.Data.PulsarApi;

namespace PulsarWorker.Client;

public class PulsarApi : IPulsarApi
{
    private readonly HttpClient _client;

    public PulsarApi(HttpClient client)
    {
        _client = client;
        _client.BaseAddress ??= new Uri("http://localhost:8080/");
    }

    public async Task<ClusterList?> GetClusters()
    {
        return await _client.GetFromJsonAsync<ClusterList>(new Uri("/admin/v2/clusters"));
    }
    
    
}