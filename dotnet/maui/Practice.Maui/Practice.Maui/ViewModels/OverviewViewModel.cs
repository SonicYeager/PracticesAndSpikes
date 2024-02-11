using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Practice.Maui.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject
{
    /// <summary>
    /// The Model date is fetched from.
    /// </summary>
    private readonly ApodModel _apodModel;

    public OverviewViewModel(ApodModel apodModel)
    {
        _apodModel = apodModel;
        Apods = [];
        Initialization = Initialize();
        SelectApogCommand = new AsyncRelayCommand(SelectApog);
    }

    public ObservableCollection<ApodViewModel> Apods { get; }
    public ICommand SelectApogCommand { get; }

    public Task Initialization { get; private set; }

    /// <summary>
    /// Initializes the Apods.
    /// </summary>
    private async Task Initialize()
    {
        try
        {
            await foreach (var apod in await Task.Run(() => Task.FromResult(_apodModel.GetLast30Apods())))
            {
                Apods.Add(apod);
            }
        }
        catch (Exception e)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make(e.Message);
            await toast.Show(cancellationTokenSource.Token);
        }
    }

    private Task SelectApog()
    {
        return Task.CompletedTask;
    }
}