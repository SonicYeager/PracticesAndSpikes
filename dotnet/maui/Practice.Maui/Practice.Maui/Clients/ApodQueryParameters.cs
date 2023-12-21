using Refit;

namespace Practice.Maui.Clients;

public sealed class ApodQueryParameters
{
    [AliasAs("date")] public string Date { get; set; }

    [AliasAs("start_date")] public string? StartDate { get; set; }

    [AliasAs("end_date")] public string EndDate { get; set; }

    [AliasAs("count")] public int? Count { get; set; }

    [AliasAs("thumbs")] public bool Thumbs { get; set; } = false;

    [AliasAs("api_key")] public required string ApiKey { get; set; } = "DEMO_KEY";
}