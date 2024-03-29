using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Practice.Maui.Application.Services;

namespace Practice.Maui.Application.Models;

public sealed class FuelBookSheetModel
{
    private readonly ISheetServiceWrapper _sheetsService;

    public FuelBookSheetModel(ISheetServiceWrapper sheetsService)
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