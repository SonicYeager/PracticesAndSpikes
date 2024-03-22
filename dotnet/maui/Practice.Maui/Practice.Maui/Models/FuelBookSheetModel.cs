﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Practice.Maui.Models;

public sealed class FuelBookSheetModel
{
    private SheetsService? _sheetsService; //TODO wrap me for code redundancy pls
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

    public async Task<IEnumerable<FuelBookRowModel>> LoadColumns()
    {
        if (_sheetsService != null)
        {
            return (await LoadSheetColumns()).Select(static rd => new FuelBookRowModel(rd));
        }

        await _initialization.WaitAsync(CancellationToken.None);
        return (await LoadSheetColumns()).Select(static rd => new FuelBookRowModel(rd));
    }

    private async Task<IEnumerable<RowData>> LoadSheetColumns()
    {
        var request = _sheetsService!.Spreadsheets.Get("1RriUAwlyEdciGOMnjuXYdUEGYuz6tY0Z-0-Q04irLLw");
        request.IncludeGridData = true;
        var sheet = await request.ExecuteAsync();
        var mainSheet = sheet.Sheets.FirstOrDefault(static s => s.Properties.Title == "Main");
        var validValues = mainSheet!.Data.First().RowData.Where(static r => r.Values[1].EffectiveValue != null).Skip(1);
        return validValues;
    }
}