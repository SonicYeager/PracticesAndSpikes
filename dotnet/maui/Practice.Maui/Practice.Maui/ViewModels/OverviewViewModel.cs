using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Practice.Maui.Application.Models;
using Practice.Maui.Views;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject, IQueryAttributable
{
    /// <summary>
    ///     The Model date is fetched from.
    /// </summary>
    private readonly FuelBookSheetModel _fuelBookSheetModel;

    private readonly IServiceProvider _serviceProvider;

    public OverviewViewModel(FuelBookSheetModel fuelBookSheetModel, IServiceProvider serviceProvider)
    {
        _fuelBookSheetModel = fuelBookSheetModel;
        _serviceProvider = serviceProvider;
        Initialization = Initialize();
        FuelStops = [];
        SelectFuelStopCommand = new AsyncRelayCommand<FuelStopEntryViewModel>(SelectFuelStopAsync);
        NewCommand = new AsyncRelayCommand(NewFuelStopEntryAsync);
    }

    public ObservableCollection<FuelStopEntryViewModel> FuelStops { get; }
    public Task Initialization { get; private set; }
    public ICommand SelectFuelStopCommand { get; }
    public ICommand NewCommand { get; }

    /// <inheritdoc />
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("deleted", out var deleted))
        {
            var matchedNote = FuelStops.FirstOrDefault(n => n.Number == int.Parse((string)deleted));

            if (matchedNote != null)
                FuelStops.Remove(matchedNote);
        }
        else if (query.TryGetValue("saved", out var saved))
        {
            var matchedNote = FuelStops.FirstOrDefault(n => n.Number == int.Parse((string)saved));

            if (matchedNote is not null)
            {
                Task.Run(async () => await matchedNote.Reload()).ConfigureAwait(false);
                FuelStops.Move(FuelStops.IndexOf(matchedNote), FuelStops.IndexOf(matchedNote));
            }

            else
            {
                var newFuelStop = _serviceProvider.GetRequiredService<FuelStopEntryViewModel>();
                newFuelStop.Number = int.Parse((string)saved);
                FuelStops.Add(newFuelStop);
                Task.Run(async () => await newFuelStop.Reload()).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///    Initializes the ViewModel.
    /// </summary>
    private async Task Initialize()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        try
        {
            foreach (var row
                     in (await _fuelBookSheetModel.LoadColumns()).Select(static c => new FuelStopEntryViewModel(c)))
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

    private static Task SelectFuelStopAsync(FuelStopEntryViewModel? fuelStop)
    {
        return fuelStop != null ? Shell.Current.GoToAsync($"{nameof(FuelStopPage)}?load={fuelStop.Number}") : Task.CompletedTask;
    }

    private static Task NewFuelStopEntryAsync()
    {
        return Shell.Current.GoToAsync($"{nameof(FuelStopPage)}?isNew=true");
    }
}