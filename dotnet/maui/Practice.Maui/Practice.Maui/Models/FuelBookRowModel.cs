using Google.Apis.Sheets.v4.Data;

namespace Practice.Maui.Models;

public sealed class FuelBookRowModel
{
    private readonly RowData? _rowData;

    public FuelBookRowModel(RowData row)
    {
        _rowData = row;
    }

    public int Number
    {
        get => (int)_rowData!.Values[0].EffectiveValue.NumberValue;
    }
    public DateOnly Date
    {
        get => DateOnly.FromDateTime(DateTime.FromOADate(_rowData!.Values[1].EffectiveValue.NumberValue.Value).Date);
    }
    public decimal CostsInEuro
    {
        get => (decimal)_rowData!.Values[2].EffectiveValue.NumberValue;
    }
    public decimal ConsumptionInLiters
    {
        get => (decimal)_rowData!.Values[3].EffectiveValue.NumberValue;
    }
    public decimal RangeInKilometers
    {
        get => (decimal)_rowData!.Values[4].EffectiveValue.NumberValue;
    }
    public decimal AverageConsumptionPerHundredKilometers
    {
        get => (decimal)_rowData!.Values[5].EffectiveValue.NumberValue;
    }
}