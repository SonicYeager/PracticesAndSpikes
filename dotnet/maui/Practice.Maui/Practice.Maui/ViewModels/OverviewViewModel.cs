using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Practice.Maui.Models;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject
{
    /// <summary>
    /// The Model date is fetched from.
    /// </summary>
    private readonly FuelBookSheetModel _fuelBookSheetModel;

    public ObservableCollection<FuelStopEntryViewModel> FuelStops { get; }
    public Task Initialization { get; private set; }
    public ICommand SelectFuelStopCommand { get; }

    public OverviewViewModel(FuelBookSheetModel fuelBookSheetModel)
    {
        _fuelBookSheetModel = fuelBookSheetModel;
        Initialization = Initialize();
        SelectFuelStopCommand = new AsyncRelayCommand<FuelStopEntryViewModel>(SelectFuelStopAsync);
        FuelStops = new();
    }

    /// <summary>
    /// Initializes the Apods.
    /// </summary>
    private async Task Initialize()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        try
        {
            foreach (var row in (await _fuelBookSheetModel.LoadColumns()).Select(static c => new FuelStopEntryViewModel(c)))
            {
                FuelStops.Add(row);
            }

            var toast = Toast.Make("Loaded Sheet Successfully");
            await toast.Show(cancellationTokenSource.Token);
            Trace.TraceInformation("Loaded Sheet Successfully");
        }
        catch (Exception e)
        {
            var toast = Toast.Make(e.Message);
            await toast.Show(cancellationTokenSource.Token);
            Trace.TraceError(e.Message);
        }
    }

    private async Task SelectFuelStopAsync(FuelStopEntryViewModel fuelStop)
    {
        //if (note != null)
        //    await Shell.Current.GoToAsync($"{nameof(NotePage)}?load={note.Identifier}");
    }
}