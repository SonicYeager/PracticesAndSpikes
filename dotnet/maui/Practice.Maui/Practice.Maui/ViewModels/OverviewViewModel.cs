using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using Practice.Maui.Models;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject
{
    /// <summary>
    /// The Model date is fetched from.
    /// </summary>
    private readonly FuelBookSheetModel _fuelBookSheetModel;

    public OverviewViewModel(FuelBookSheetModel fuelBookSheetModel)
    {
        _fuelBookSheetModel = fuelBookSheetModel;
        Initialization = Initialize();
    }

    public Task Initialization { get; private set; }

    /// <summary>
    /// Initializes the Apods.
    /// </summary>
    private async Task Initialize()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        try
        {
            await _fuelBookSheetModel.LoadSheet();

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
}