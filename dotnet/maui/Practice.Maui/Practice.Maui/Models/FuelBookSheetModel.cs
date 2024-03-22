using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace Practice.Maui.Models;

public sealed class FuelBookSheetModel
{
    private SheetsService? _sheetsService;
    private readonly Task _initialization;

    public FuelBookSheetModel()
    {
        _initialization = Task.Run(async () =>
        {
            var stream = await FileSystem.Current.OpenAppPackageFileAsync("googleapi.json");
            var scopes = new[]
            {
                SheetsService.Scope.Spreadsheets,
            };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(FileSystem.Current.AppDataDirectory));

            _sheetsService = new(new()
            {
                HttpClientInitializer = credential, ApplicationName = "Practice.Maui",
            });
        });
    }

    public async Task LoadSheet()
    {
        if (_sheetsService != null)
        {
            var sheet = await _sheetsService.Spreadsheets.Get("1RriUAwlyEdciGOMnjuXYdUEGYuz6tY0Z-0-Q04irLLw").ExecuteAsync();
            var range = sheet.Sheets;
        }
        else
        {
            await _initialization.WaitAsync(CancellationToken.None);
            var sheet = await _sheetsService.Spreadsheets.Get("1RriUAwlyEdciGOMnjuXYdUEGYuz6tY0Z-0-Q04irLLw").ExecuteAsync();
            var range = sheet.Sheets;
        }
    }
}