using Practice.Maui.Models;

namespace Practice.Maui.ViewModels;

public sealed class FuelStopEntryViewModel
{
    private readonly FuelBookRowModel _fuelBookRowModel;

    public FuelStopEntryViewModel(FuelBookRowModel fuelBookRowModel)
    {
        _fuelBookRowModel = fuelBookRowModel;
    }

    public int Number
    {
        get => _fuelBookRowModel.Number;
    }
    public string Date
    {
        get => _fuelBookRowModel.Date.ToShortDateString();
    }
    public decimal CostsInEuro
    {
        get => _fuelBookRowModel.CostsInEuro;
    }
    public decimal ConsumptionInLiters
    {
        get => _fuelBookRowModel.ConsumptionInLiters;
    }
    public decimal RangeInKilometers
    {
        get => _fuelBookRowModel.RangeInKilometers;
    }
    public decimal AverageConsumptionPerHundredKilometers
    {
        get => _fuelBookRowModel.AverageConsumptionPerHundredKilometers;
    }
}