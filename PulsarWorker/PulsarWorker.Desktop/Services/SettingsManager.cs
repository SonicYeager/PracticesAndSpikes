using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulsarWorker.Data.Entities;
using PulsarWorker.Database.Context;
using PulsarWorker.Database.Extensions;

namespace PulsarWorker.Desktop.Services;

public static class AvailableSettings
{
    public const string PulsarHostOptionKey = "Pulsar Host";
    public const string AppThemeOptionKey = "App Theme";
}

public sealed class SettingsManager
{
    private readonly DbContextOptions<PulsarWorkerDbContext> _dbContextOptions;
    private readonly UserManager _userManager;

    public SettingsManager(DbContextOptions<PulsarWorkerDbContext> dbContextOptions, UserManager userManager)
    {
        _dbContextOptions = dbContextOptions;
        _userManager = userManager;
        OnSettingChanged = (_, _) =>
        {
            _activeSettings = null;
        };
    }

    public event Action<string, object?>? OnSettingChanged;

    private ICollection<KeyValuePair<string, object?>>? _activeSettings;
    public Task<ICollection<KeyValuePair<string, object?>>> ActiveSettings
    {
        get => _activeSettings is null ? FetchLatestSettings() : Task.FromResult(_activeSettings);
    }

    private async Task<ICollection<KeyValuePair<string, object?>>> FetchLatestSettings()
    {
        await using var context = Repository.Connect(_dbContextOptions);
        var settingsEntities = context.Set<SettingsEntity>().Where(s => s.UserId == _userManager.CurrentUserId);
        var settings = new List<KeyValuePair<string, object?>>();
        foreach (var setting in settingsEntities)
        {
            settings.Add(new(setting.Key, setting.Value));
        }

        return settings;
    }

    public void EmitSettingChanged(string key, object? value)
    {
        OnSettingChanged?.Invoke(key, value);
    }
}