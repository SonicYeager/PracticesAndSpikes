using Refit;

namespace Practice.Maui.Clients;

public interface INasaApodApi
{
    [Get("/planetary/apod")]
    Task<AstronomyPictureOfTheDay> GetApod(ApodQueryParameters queryParameters);
}