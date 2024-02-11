using Refit;

namespace Practice.Maui.Clients;

public interface INasaApodApi
{
    [Get("/planetary/apod")]
    Task<IEnumerable<AstronomyPictureOfTheDay>> GetApods(ApodQueryParameters queryParameters);
}