using System;
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

    public async Task GetPersistedSettings(ObservableCollection<object> observableCollection, Func<Task> onSuccess, int userId = 1)
    {
        await using var context = Repository.Connect(_dbContextOptions);
        var settingsEntities = context.Set<SettingsEntity>().Where(s => s.UserId == userId);
        foreach (var settingsEntity in settingsEntities)
        {
            if (settingsEntity.Key == "Pulsar Host")
            {
                var textSetting = CreateTextSetting("Pulsar Host", settingsEntity.Value, onSuccess);
                observableCollection.Add(textSetting);
            }
        }
    }

    private TextSetting CreateTextSetting(string host, string? value, Func<Task> onSuccess)
    {
        var textSettingViewModel = new TextSettingViewModel(host, async (key, newValue) => await PersistSetting(key, newValue, onSuccess))
        {
            Text = value,
        };

        return new()
        {
            DataContext = textSettingViewModel,
        };
    }

    private async Task PersistSetting(string key, object? value, Func<Task> onSuccess, int userId = 1)
    {
        await using var context = Repository.Connect(_dbContextOptions);

        var existing = await context.Set<SettingsEntity>().FirstAsync(s => s.UserId == userId && s.Key == key);
        existing.Value = value as string;
        await context.SaveChangesAsync();
        await onSuccess();
    }
}