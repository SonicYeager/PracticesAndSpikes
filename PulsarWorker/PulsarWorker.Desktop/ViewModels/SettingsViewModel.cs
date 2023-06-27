using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PulsarWorker.Desktop.Models;

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