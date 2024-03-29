using Google.Apis.Sheets.v4.Data;

namespace Practice.Maui.Tests;

internal static class TestDataProvider
{
    public static Sheet CreateMainSheet(int rowNumber)
    {
        return new()
        {
            Data = new List<GridData>
            {
                new()
                {
                    RowData = new List<RowData>
                    {
                        new()
                        {
                            Values = new List<CellData>
                            {
                                new()
                                {
                                    EffectiveValue = new()
                                    {
                                        NumberValue = rowNumber,
                                    },
                                },
                            },
                        },
                    },
                },
            },
        };
    }

    public static RowData CreateRowData(
        int number,
        DateTime date,
        decimal costsInEuro,
        decimal consumptionInLiters,
        decimal rangeInKilometers,
        decimal averageConsumptionPerHundredKilometers)
    {
        return new()
        {
            Values = new List<CellData>
            {
                new()
                {
                    EffectiveValue = new()
                    {
                        NumberValue = number,
                    },
                },
                new()
                {
                    EffectiveValue = new()
                    {
                        NumberValue = date.ToOADate(),
                    },
                },
                new()
                {
                    EffectiveValue = new()
                    {
                        NumberValue = (double)costsInEuro,
                    },
                },
                new()
                {
                    EffectiveValue = new()
                    {
                        NumberValue = (double)consumptionInLiters,
                    },
                },
                new()
                {
                    EffectiveValue = new()
                    {
                        NumberValue = (double)rangeInKilometers,
                    },
                },
                new()
                {
                    EffectiveValue = new()
                    {
                        NumberValue = (double)averageConsumptionPerHundredKilometers,
                    },
                },
            },
        };
    }
}