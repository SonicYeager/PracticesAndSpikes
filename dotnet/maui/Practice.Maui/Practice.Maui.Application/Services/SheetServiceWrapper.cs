using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Practice.Maui.Application.Services;

public sealed class SheetServiceWrapper : ISheetServiceWrapper
{
    private SheetsService _sheetsService;
    private readonly IFileSystem _fileSystem;
    public Task Initialization { get; }

    public SheetServiceWrapper(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        Initialization = Task.Run(async () =>
        {
            var stream = await _fileSystem.OpenReadAsync("googleapi.json");
            var scopes = new[]
            {
                SheetsService.Scope.Spreadsheets,
            };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(_fileSystem.AppDataDirectory));

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