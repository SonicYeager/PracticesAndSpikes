using System.Net.Http.Json;

namespace PulsarWorker.Client;

public class PulsarClient : IPulsarClient
{
    private readonly HttpClient _client;

    public PulsarClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress ??= new Uri("http://localhost:8080");
    }

    public async Task<IEnumerable<string>?> GetClusters()
    {
        try
        {
            return await _client.GetFromJsonAsync<IEnumerable<string>>(new Uri("/admin/v2/clusters", UriKind.Relative));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<IEnumerable<string>?> GetTenants()
    {
        try
        {
            return await _client.GetFromJsonAsync<IEnumerable<string>>(new Uri("/admin/v2/tenants", UriKind.Relative));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<IEnumerable<string>?> GetNamespaces(string tenant)
    {
        try
        {
            return await _client.GetFromJsonAsync<IEnumerable<string>>(new Uri($"/admin/v2/namespaces/{tenant}",
                UriKind.Relative));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<IEnumerable<string>?> GetTopics(string tenant, string pulsarNamespace)
    {
        try
        {
            return await _client.GetFromJsonAsync<IEnumerable<string>>(new Uri($"/persistent/{tenant}/{pulsarNamespace}",
                UriKind.Relative));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<HttpResponseMessage> DeleteTopic(string tenant, string pulsarNamespace, string topic)
    {
        return await _client.DeleteAsync(new Uri($"/persistent/{tenant}/{pulsarNamespace}/{topic}", UriKind.Relative));
    }
}