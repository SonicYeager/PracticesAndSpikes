namespace MauiAppTesty.Dto;

internal sealed record ChuckNorrisJokeDto(
    string id,
    IEnumerable<string> categories,
    string created_at,
    string updated_at,
    Uri icon_url,
    Uri url,
    string value);