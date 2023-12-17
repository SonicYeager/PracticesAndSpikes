namespace Practice.Maui.Clients;

public class ApodQueryParameters
{
    public DateTime Date { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Count { get; set; }
    public bool Thumbs { get; set; }
    public string ApiKey { get; set; }
}