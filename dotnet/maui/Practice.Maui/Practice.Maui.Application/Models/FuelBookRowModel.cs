using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;
using Practice.Maui.Application.Services;

namespace Practice.Maui.Application.Models;

public sealed class FuelBookRowModel
{
    private readonly ISheetServiceWrapper _sheetService;

    public FuelBookRowModel(ISheetServiceWrapper sheetService)
    {
        _sheetService = sheetService;
    }

    public RowData? RowData { get; set; }
    public int Number
    {
        get => (int?)RowData?.Values[0].EffectiveValue.NumberValue ?? 0;
        set => RowData!.Values[0].EffectiveValue.NumberValue = (double)value;
    }
    public DateOnly Date
    {
        get => DateOnly.FromDateTime(DateTime.FromOADate(RowData?.Values[1].EffectiveValue.NumberValue.Value ?? 0).Date);
        set => RowData!.Values[1].EffectiveValue.NumberValue = value.ToDateTime(TimeOnly.MinValue).ToOADate();
    }
    public decimal CostsInEuro
    {
        get => (decimal?)RowData?.Values[2].EffectiveValue.NumberValue ?? 0;
        set => RowData!.Values[2].EffectiveValue.NumberValue = (double)value;
    }
    public decimal ConsumptionInLiters
    {
        get => (decimal?)RowData?.Values[3].EffectiveValue.NumberValue ?? 0;
        set => RowData!.Values[3].EffectiveValue.NumberValue = (double)value;
    }
    public decimal RangeInKilometers
    {
        get => (decimal?)RowData?.Values[4].EffectiveValue.NumberValue ?? 0;
        set => RowData!.Values[4].EffectiveValue.NumberValue = (double)value;
    }
    public decimal AverageConsumptionPerHundredKilometers
    {
        get => (decimal?)RowData?.Values[5].EffectiveValue.NumberValue ?? 0;
        set => RowData!.Values[5].EffectiveValue.NumberValue = (double)value;
    }

    public async Task Load(int rowNumber)
    {
        var mainSheet = await _sheetService.GetMainSheet();
        RowData = mainSheet!.Data.First().RowData
            .FirstOrDefault(r => r.Values[0].EffectiveValue != null && (int?)r.Values[0].EffectiveValue.NumberValue == rowNumber);
    }
}