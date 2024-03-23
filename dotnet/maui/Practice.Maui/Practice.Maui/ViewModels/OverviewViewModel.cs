using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Practice.Maui.Models;
using Practice.Maui.Views;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject, IQueryAttributable
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

    /// <inheritdoc />
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        //if (query.TryGetValue("deleted", out var deleted))
        //{
        //    var noteId = deleted.ToString();
        //    var matchedNote = FuelStops.FirstOrDefault(n => n.Identifier == noteId);
        //
        //    // If note exists, delete it
        //    if (matchedNote != null)
        //        FuelStops.Remove(matchedNote);
        //}
        //else if (query.TryGetValue("saved", out var saved))
        //{
        //    var noteId = saved.ToString();
        //    var matchedNote = FuelStops.FirstOrDefault(n => n.Identifier == noteId);
        //
        //    // If note is found, update it
        //    if (matchedNote != null)
        //    {
        //        matchedNote.Reload();
        //        FuelStops.Move(FuelStops.IndexOf(matchedNote), 0);
        //    }
        //
        //    // If note isn't found, it's new; add it.
        //    else
        //    {
        //        FuelStops.Insert(0, new(Note.Load(noteId)));
        //    }
        //}
    }

    private static async Task SelectFuelStopAsync(FuelStopEntryViewModel fuelStop)
    {
        if (fuelStop != null)
            await Shell.Current.GoToAsync($"{nameof(FuelStopPage)}?load={fuelStop.Number}");
    }
}