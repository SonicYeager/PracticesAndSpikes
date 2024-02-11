namespace Practice.Maui.ViewModels;

public sealed record ApodViewModel
{
    public string Copyright { get; set; }
    public DateTime Date { get; set; }
    public ImageSource Image { get; set; }
    public string MediaType { get; set; }
    public string ServiceVersion { get; set; }
    public string Title { get; set; }
}