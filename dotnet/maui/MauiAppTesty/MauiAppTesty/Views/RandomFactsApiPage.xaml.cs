using MauiAppTesty.Dto;
using MauiAppTesty.ViewModels;
using System.Text.Json;

namespace MauiAppTesty.Views;

public sealed partial class RandomFactsApiPage : ContentPage
{
	private readonly HttpClient _httpClient;
	private readonly RandomFactsApiPageViewModel _randomFactsApiPageViewModel;
    private readonly JsonSerializerOptions _serializerOptions;

    public RandomFactsApiPage(HttpClient httpClient, RandomFactsApiPageViewModel randomFactsApiPageViewModel)
	{
		InitializeComponent();

		_httpClient = httpClient;
        _randomFactsApiPageViewModel = randomFactsApiPageViewModel;

        _httpClient.BaseAddress = new Uri("https://api.chucknorris.io/");
        _randomFactsApiPageViewModel.FetchNew = new Command(async () => await FetchNew());
        _serializerOptions = new JsonSerializerOptions
        {};

        BindingContext = _randomFactsApiPageViewModel;
	}

	private async Task FetchNew()
	{
		var request = new Uri("/jokes/random?category=dev", UriKind.Relative);
		var response = await _httpClient.GetAsync(request);
		var str = await response.Content.ReadAsStringAsync();
		var dto = JsonSerializer.Deserialize<ChuckNorrisJokeDto>(str, _serializerOptions);
        _randomFactsApiPageViewModel.Joke = dto.value;
		_randomFactsApiPageViewModel.Categories = string.Join(" ", dto.categories);
		_randomFactsApiPageViewModel.Id = dto.id;
		_randomFactsApiPageViewModel.Url = dto.url;
		_randomFactsApiPageViewModel.UpdatedAt = DateTime.Parse(dto.updated_at);
		_randomFactsApiPageViewModel.CreatedAt = DateTime.Parse(dto.created_at);
		_randomFactsApiPageViewModel.IconUrl = dto.icon_url;

    }
}