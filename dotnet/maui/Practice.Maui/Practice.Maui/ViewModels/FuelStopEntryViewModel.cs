using CommunityToolkit.Mvvm.ComponentModel;
using Practice.Maui.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Practice.Maui.ViewModels;

public sealed class FuelStopEntryViewModel : ObservableObject, IQueryAttributable
{
    private readonly FuelBookRowModel _fuelBookRowModel;

    public FuelStopEntryViewModel(FuelBookRowModel fuelBookRowModel)
    {
        _fuelBookRowModel = fuelBookRowModel;
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(Delete);
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public int Number
    {
        get => _fuelBookRowModel.Number;
        set
        {
            if (_fuelBookRowModel.Number != value)
            {
                _fuelBookRowModel.Number = value;
                OnPropertyChanged();
            }
        }
    }
    public string Date
    {
        get => _fuelBookRowModel.Date.ToShortDateString();
        set
        {
            if (_fuelBookRowModel.Date != DateOnly.Parse(value))
            {
                _fuelBookRowModel.Date = DateOnly.Parse(value);
                OnPropertyChanged();
            }
        }
    }
    public decimal CostsInEuro
    {
        get => _fuelBookRowModel.CostsInEuro;
        set
        {
            if (_fuelBookRowModel.CostsInEuro != value)
            {
                _fuelBookRowModel.CostsInEuro = value;
                OnPropertyChanged();
            }
        }
    }
    public decimal ConsumptionInLiters
    {
        get => _fuelBookRowModel.ConsumptionInLiters;
        set
        {
            if (_fuelBookRowModel.ConsumptionInLiters != value)
            {
                _fuelBookRowModel.ConsumptionInLiters = value;
                OnPropertyChanged();
            }
        }
    }
    public decimal RangeInKilometers
    {
        get => _fuelBookRowModel.RangeInKilometers;
        set
        {
            if (_fuelBookRowModel.RangeInKilometers != value)
            {
                _fuelBookRowModel.RangeInKilometers = value;
                OnPropertyChanged();
            }
        }
    }
    public decimal AverageConsumptionPerHundredKilometers
    {
        get => _fuelBookRowModel.AverageConsumptionPerHundredKilometers;
        set
        {
            if (_fuelBookRowModel.AverageConsumptionPerHundredKilometers != value)
            {
                _fuelBookRowModel.AverageConsumptionPerHundredKilometers = value;
                OnPropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("load", out var value))
        {
            Task.Run(async () =>
            {
                await _fuelBookRowModel.Load(int.Parse((string)value));
                RefreshProperties();
            });
        }
    }

    private async Task Save()
    {
        //_fuelBookRowModel.Save();
        //await Shell.Current.GoToAsync($"..?saved={_fuelBookRowModel.Number}");
    }

    private async Task Delete()
    {
        //_fuelBookRowModel.Delete();
        //await Shell.Current.GoToAsync($"..?deleted={_fuelBookRowModel.Number}");
    }

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Number));
        OnPropertyChanged(nameof(Date));
        OnPropertyChanged(nameof(CostsInEuro));
        OnPropertyChanged(nameof(ConsumptionInLiters));
        OnPropertyChanged(nameof(RangeInKilometers));
        OnPropertyChanged(nameof(AverageConsumptionPerHundredKilometers));
    }
}