using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Practice.Maui.Services;

public class SheetServiceWrapper
{
    private SheetsService _sheetsService;
    public Task Initialization { get; }

    public SheetServiceWrapper()
    {
        Initialization = Task.Run(async () =>
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

    public async Task<Sheet?> GetMainSheet()
    {
        if (!Initialization.IsCompleted)
        {
            await Initialization.WaitAsync(CancellationToken.None);
        }

        var request = _sheetsService.Spreadsheets.Get("1RriUAwlyEdciGOMnjuXYdUEGYuz6tY0Z-0-Q04irLLw");
        request.IncludeGridData = true;
        var sheet = await request.ExecuteAsync();
        return sheet.Sheets.FirstOrDefault(static s => s.Properties.Title == "Main");
    }
}