using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private const string SheetId = "1F6RxzQ8J88yf_d5d08Y77THMjYgewPP6yUfQOvhLXIY";

    private readonly IFileSystem _fileSystem;
    private SheetsService _sheetsService;

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
    public Task Initialization { get; }

    public async Task<Sheet?> GetMainSheet()
    {
        if (!Initialization.IsCompleted) await Initialization.WaitAsync(CancellationToken.None);

        var request = _sheetsService.Spreadsheets.Get(SheetId);
        request.IncludeGridData = true;
        var sheet = await request.ExecuteAsync();
        return sheet.Sheets.FirstOrDefault(static s => s.Properties.Title == "Main");
    }

    public async Task UpdateRow(RowData rowData)
    {
        var mainSheet = await GetMainSheet();
        var update = new BatchUpdateSpreadsheetRequest
        {
            Requests = new List<Request>
            {
                new()
                {
                    UpdateCells = new()
                    {
                        Fields = "*",
                        Range = new()
                        {
                            SheetId = GetSheetId(mainSheet),
                            StartRowIndex = GetRowDataIndex(rowData, mainSheet),
                            EndRowIndex = GetRowDataIndex(rowData, mainSheet) + 1,
                        },
                        Rows = new List<RowData>
                        {
                            rowData,
                        },
                    },
                },
            },
        };
        var request = _sheetsService.Spreadsheets.BatchUpdate(update, SheetId);
        try
        {
            _ = await request.ExecuteAsync();
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }
    }

    private static int GetRowDataIndex(RowData rowData, Sheet? mainSheet)
    {
        var first = mainSheet.Data.First().RowData
            .First(rd => rd.Values[0].EffectiveValue.NumberValue == rowData.Values[0].EffectiveValue.NumberValue);
        var index = mainSheet.Data.First().RowData.IndexOf(first);
        return index;
    }

    private static int? GetSheetId(Sheet? mainSheet)
    {
        return mainSheet.Properties.SheetId;
    }
}