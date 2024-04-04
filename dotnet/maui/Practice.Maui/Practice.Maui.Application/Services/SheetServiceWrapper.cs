using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Practice.Maui.Application.Services;

public sealed class SheetServiceWrapper : ISheetServiceWrapper
{
    private const string SheetId = "1F6RxzQ8J88yf_d5d08Y77THMjYgewPP6yUfQOvhLXIY";

    private SheetsService? _sheetsService;
    private UserCredential? _credential;

    public SheetServiceWrapper(ISheetConfigProvider sheetConfigProvider, ICodeReceiver codeReceiver)
    {
        Initialization = Task.Run(async () =>
        {
            var scopes = new[]
            {
                SheetsService.Scope.Spreadsheets,
            };

            _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                await sheetConfigProvider.GetClientSecrets(),
                scopes,
                "user",
                CancellationToken.None,
                sheetConfigProvider.FileDataStore,
                codeReceiver);

            _sheetsService = new(new()
            {
                HttpClientInitializer = _credential, ApplicationName = "Practice.Maui",
            });
        });
    }

    public Task Initialization { get; }

    public async Task<Sheet> GetMainSheet()
    {
        if (!Initialization.IsCompleted) await Initialization.WaitAsync(CancellationToken.None);

        var request = _sheetsService!.Spreadsheets.Get(SheetId);
        request.IncludeGridData = true;
        var sheet = await HandleRequest<Spreadsheet, SpreadsheetsResource.GetRequest>(request);
        return sheet.Sheets.First(static s => s.Properties.Title == "Main");
    }

    public async Task UpdateRow(RowData rowData)
    {
        var mainSheet = await GetMainSheet();
        var update = CreateBatchUpdateSpreadsheetRequest(rowData, mainSheet);
        if (!Initialization.IsCompleted) await Initialization.WaitAsync(CancellationToken.None);
        var request = _sheetsService!.Spreadsheets.BatchUpdate(update, SheetId);
        try
        {
            _ = await HandleRequest<BatchUpdateSpreadsheetResponse, SpreadsheetsResource.BatchUpdateRequest>(request);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }
    }

    private static BatchUpdateSpreadsheetRequest CreateBatchUpdateSpreadsheetRequest(RowData rowData, Sheet mainSheet)
    {
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
                            StartRowIndex = GetRowDataIndex(rowData, mainSheet) ?? GetNewRowDataIndex(mainSheet),
                            EndRowIndex = (GetRowDataIndex(rowData, mainSheet) ?? GetNewRowDataIndex(mainSheet)) + 1,
                        },
                        Rows = new List<RowData>
                        {
                            rowData,
                        },
                    },
                },
            },
        };
        return update;
    }

    private static int? GetRowDataIndex(RowData rowData, Sheet mainSheet)
    {
        const double tolerance = 0.0001;
        var first = mainSheet.Data.First().RowData
            .FirstOrDefault(rd
                => rd.Values[0].EffectiveValue.NumberValue is not null &&
                   rowData.Values[0].EffectiveValue.NumberValue is not null &&
                   Math.Abs(rd.Values[0].EffectiveValue.NumberValue!.Value - rowData.Values[0].EffectiveValue.NumberValue!.Value) <
                   tolerance);
        if (first is null) return null;

        return mainSheet.Data.First().RowData.IndexOf(first);
    }

    private static int GetNewRowDataIndex(Sheet mainSheet)
    {
        var first = mainSheet.Data.First().RowData.IndexOf(mainSheet.Data.First().RowData.Last());
        return first + 1;
    }

    private static int? GetSheetId(Sheet? mainSheet)
    {
        return mainSheet?.Properties.SheetId;
    }

    private async Task<TResponse> HandleRequest<TResponse, TRequest>(TRequest request) where TRequest : SheetsBaseServiceRequest<TResponse>
    {
        try
        {
            return await request.ExecuteAsync();
        }
        catch (HttpRequestException e) when (e.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden)
        {
            Trace.TraceWarning($"User Grant Expired with error: {e.Message}");
            await Reauthorize();
            return await request.ExecuteAsync();
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
            throw;
        }
    }

    private Task Reauthorize()
    {
        return GoogleWebAuthorizationBroker.ReauthorizeAsync(_credential, CancellationToken.None);
    }
}