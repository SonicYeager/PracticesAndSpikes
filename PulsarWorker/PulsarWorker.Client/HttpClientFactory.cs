namespace PulsarWorker.Client;

public class HttpClientFactory
{
    public HttpClient GetHttpClient(Uri baseAddress)
    {
        return new()
        {
            BaseAddress = baseAddress,
        };
    }
}