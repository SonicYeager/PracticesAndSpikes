using System;
using System.Diagnostics;
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
        set
        {
            if (RowData is not null)
            {
                RowData!.Values[0].EffectiveValue.NumberValue = (double)value;
                RowData!.Values[0].UserEnteredValue.NumberValue = (double)value;
            }
        }
    }
    public DateOnly Date
    {
        get
        {
            var dateTime = DateTime.FromOADate(RowData?.Values[1].EffectiveValue.NumberValue ?? DateTime.Today.ToOADate());
            if (dateTime <= DateTime.MinValue || dateTime <= DateTime.Parse("2020-01-01"))
                dateTime = DateTime.Today;
            return DateOnly.FromDateTime(dateTime);
        }
        set
        {
            if (RowData is not null)
            {
                try
                {
                    RowData.Values[1].EffectiveValue.NumberValue = value.ToDateTime(TimeOnly.MinValue).ToOADate();
                    RowData.Values[1].UserEnteredValue.NumberValue = value.ToDateTime(TimeOnly.MinValue).ToOADate();
                }
                catch (Exception e)
                {
                    Trace.TraceWarning(e.ToString());
                }
            }
        }
    }
    public decimal CostsInEuro
    {
        get => (decimal?)RowData?.Values[2].EffectiveValue.NumberValue ?? 0;
        set
        {
            if (RowData is not null)
            {
                RowData!.Values[2].EffectiveValue.NumberValue = (double)value;
                RowData!.Values[2].UserEnteredValue.NumberValue = (double)value;
            }
        }
    }
    public decimal ConsumptionInLiters
    {
        get => (decimal?)RowData?.Values[3].EffectiveValue.NumberValue ?? 0;
        set
        {
            if (RowData is not null)
            {
                RowData!.Values[3].EffectiveValue.NumberValue = (double)value;
                RowData!.Values[3].UserEnteredValue.NumberValue = (double)value;
            }
        }
    }
    public decimal RangeInKilometers
    {
        get => (decimal?)RowData?.Values[4].EffectiveValue.NumberValue ?? 0;
        set
        {
            if (RowData is not null)
            {
                RowData!.Values[4].EffectiveValue.NumberValue = (double)value;
                RowData!.Values[4].UserEnteredValue.NumberValue = (double)value;
            }
        }
    }
    public decimal AverageConsumptionPerHundredKilometers
    {
        get => (decimal?)RowData?.Values[5].EffectiveValue.NumberValue ?? 0;
    }

    public async Task Load(int rowNumber)
    {
        var sheet = await _sheetService.GetMainSheet();
        RowData = sheet?.Data.First().RowData
            .FirstOrDefault(r => r.Values[0].EffectiveValue != null && (int?)r.Values[0].EffectiveValue.NumberValue == rowNumber);
    }

    public Task Save()
    {
        return RowData is not null ? _sheetService.UpdateRow(RowData) : Task.CompletedTask;
    }

    public async Task New()
    {
        var sheet = await _sheetService.GetMainSheet();
        var nextRow = sheet?.Data.First().RowData
            .Skip(1)
            .First(static r => r.Values[1].EffectiveValue == null);
        var nextRowNumber = sheet?.Data.First().RowData
            .Skip(1)
            .Where(static r => r.Values[1].EffectiveValue != null)
            .Select(static r => (int?)r.Values[0].EffectiveValue.NumberValue)
            .Max() + 1;

        if (nextRow is null)
            throw new InvalidOperationException("No empty row found.");

        nextRow.Values[0].EffectiveValue = new()
        {
            NumberValue = nextRowNumber,
        };
        nextRow.Values[0].UserEnteredValue = new()
        {
            NumberValue = nextRowNumber,
        };

        foreach (var value in nextRow.Values)
        {
            value.UserEnteredValue ??= new()
            {
                NumberValue = 0,
            };
            value.EffectiveValue ??= new()
            {
                NumberValue = 0,
            };
        }

        RowData = nextRow;
    }
}