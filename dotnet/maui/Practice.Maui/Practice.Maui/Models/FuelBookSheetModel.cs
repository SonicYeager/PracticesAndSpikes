using Practice.Maui.Services;

namespace Practice.Maui.Models;

public sealed class FuelBookSheetModel
{
    private readonly SheetServiceWrapper _sheetsService;

    public FuelBookSheetModel(SheetServiceWrapper sheetsService)
    {
        _sheetsService = sheetsService;
    }

    public async Task<IEnumerable<FuelBookRowModel>> LoadColumns()
    {
        var mainSheet = await _sheetsService.GetMainSheet();
        return mainSheet!.Data.First().RowData
            .Where(static r => r.Values[1].EffectiveValue != null)
            .Skip(1)
            .Select(rd => new FuelBookRowModel(_sheetsService)
            {
                RowData = rd,
            });
    }
}