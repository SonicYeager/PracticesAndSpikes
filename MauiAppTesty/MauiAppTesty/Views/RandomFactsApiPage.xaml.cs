using MauiAppTesty.Dto;
using MauiAppTesty.ViewModels;
using Swashbuckle.Swagger;

namespace MauiAppTesty.Views;

public partial class RandomFactsApiPage : ContentPage
{
	private readonly HttpClient _httpClient = new HttpClient();
	private readonly RandomFactsApiPageViewModel _randomFactsApiPageViewModel = new();

	public RandomFactsApiPage()
	{
		InitializeComponent();
		_httpClient.BaseAddress = new Uri("https://api.chucknorris.io/");
        _randomFactsApiPageViewModel.FetchNew = new Command(async () => await FetchNew());
        BindingContext = _randomFactsApiPageViewModel;
	}

	private async Task FetchNew()
	{
		var request = new Uri("/jokes/random?category=dev", UriKind.Relative);
		var response = await _httpClient.GetAsync(request);
		var dto = await response.Content.ReadAsAsync<ChuckNorrisJokeDto>();
		var str = await response.Content.ReadAsStringAsync();
		_randomFactsApiPageViewModel.Joke = dto.Value;
		_randomFactsApiPageViewModel.Categories = string.Join(" ", dto.Categories);
		_randomFactsApiPageViewModel.Id = dto.Id;
		_randomFactsApiPageViewModel.Url = dto.Url;
		_randomFactsApiPageViewModel.UpdatedAt = dto.Updated_At;
		_randomFactsApiPageViewModel.CreatedAt = dto.Created_At;
		_randomFactsApiPageViewModel.IconUrl = dto.Icon_Url;

    }
}