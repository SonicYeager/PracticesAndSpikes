using Practice.Maui.Clients;
using Practice.Maui.ViewModels;

namespace Practice.Maui.Models;

public sealed class ApodModel
{
    private readonly INasaApodApi _nasaApodApi;

    public ApodModel(INasaApodApi nasaApodApi)
    {
        _nasaApodApi = nasaApodApi;
    }

    public async IAsyncEnumerable<ApodViewModel> GetLast30Apods()
    {
        var apods = await _nasaApodApi.GetApods(new()
        {
            ApiKey = "DEMO_KEY",
            StartDate = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd"),
            Thumbs = true,
        });

        foreach (var apod in apods)
        {
            yield return new ApodViewModel
            {
                Copyright = apod.Copyright,
                Url = apod.Url,
                HdUrl = apod.HdUrl,
                MediaType = apod.MediaType,
                ServiceVersion = apod.ServiceVersion,
                Date = apod.Date,
                Title = apod.Title,
            };
        }
    }
}