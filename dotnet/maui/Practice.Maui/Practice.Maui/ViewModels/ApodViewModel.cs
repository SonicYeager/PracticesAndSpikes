namespace Practice.Maui.ViewModels;

public sealed record ApodViewModel
{
    public string Copyright { get; set; }
    public DateTime Date { get; set; }
    public string HdUrl { get; set; }
    public string Url { get; set; }
    public string MediaType { get; set; }
    public string ServiceVersion { get; set; }
    public string Title { get; set; }
}