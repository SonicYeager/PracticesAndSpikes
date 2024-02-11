using Practice.Maui.Clients;
using Practice.Maui.ViewModels;

namespace Practice.Maui.Models;

public sealed class ApodModel(INasaApodApi nasaApodApi)
{

    public async IAsyncEnumerable<ApodViewModel> GetLast30Apods()
    {
        var apods = await nasaApodApi.GetApods(new()
        {
            ApiKey = "DEMO_KEY",
            StartDate = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd"),
            Thumbs = true,
        });

        foreach (var apod in apods)
        {
            yield return new()
            {
                Copyright = apod.Copyright,
                Image = ImageSource.FromUri(new(apod.HdUrl ?? apod.Url)),
                MediaType = apod.MediaType,
                ServiceVersion = apod.ServiceVersion,
                Date = apod.Date,
                Title = apod.Title,
            };
        }
    }
}