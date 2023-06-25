using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using PulsarWorker.Desktop.Models;
using PulsarWorker.Desktop.Views.Components;

namespace PulsarWorker.Desktop.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly SettingsModel _model;

    public SettingsViewModel(SettingsModel model)
    {
        _model = model;
    }

    public async Task LoadAsync()
    {
        await _model.GetPersistedSettings(PersistedOptions,
            1); // userId 1 is the test user predefined by the database project
    }

    public ObservableCollection<object> PersistedOptions { get; init; } = new();
}