namespace PulsarWorker.Client;

public interface IPulsarApi
{
    public Task<IEnumerable<string>?> GetClusters();
    
    public Task<IEnumerable<string>?> GetNamespaces(string tenant);

    public Task<IEnumerable<string>?> GetTopics(string tenant, string pulsarNamespace);

    public Task<HttpResponseMessage> DeleteTopic(string tenant, string pulsarNamespace, string topic);
}