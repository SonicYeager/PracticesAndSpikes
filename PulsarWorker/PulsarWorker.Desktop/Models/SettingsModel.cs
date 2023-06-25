using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulsarWorker.Data.Entities;
using PulsarWorker.Database.Context;
using PulsarWorker.Database.Extensions;
using PulsarWorker.Desktop.ViewModels.Components;
using PulsarWorker.Desktop.Views.Components;

namespace PulsarWorker.Desktop.Models;

public sealed class SettingsModel
{
    private readonly DbContextOptions<PulsarWorkerDbContext> _dbContextOptions;

    public SettingsModel(DbContextOptions<PulsarWorkerDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task GetPersistedSettings(ObservableCollection<object> observableCollection, int userId)
    {
        await using var context = Repository.Connect(_dbContextOptions);
        var settingsEntities = context.Set<SettingsEntity>().Where(s => s.UserId == userId);
        foreach (var settingsEntity in settingsEntities)
        {
            object setting = null!;

            if (settingsEntity.Key == "Pulsar Host")
            {
                setting = new TextSetting
                {
                    DataContext = new TextSettingViewModel("Pulsar Host")
                    {
                        Text = settingsEntity.Value,
                    },
                };
            }

            observableCollection.Add(setting);
        }
    }
}