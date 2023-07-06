﻿using System.Linq;
using System.Net.Http;

namespace PulsarWorker.Desktop.Services;

public class SettingsDependentHttpClient : HttpClient 
    //TODO sadly BaseAddress cannot be changed after first request -> supply new client internally (or wrap including interface provided by PulsarWorker.Client)?
{
    public SettingsDependentHttpClient(SettingsManager settingsManager)
    {
        settingsManager.OnSettingChanged += (key, value) =>
        {
            if (key == "Pulsar Host")
                BaseAddress = new(value as string ?? "http://localhost:8080");
        };
        SetBaseAddressAsync(settingsManager);
    }

    private async void SetBaseAddressAsync(SettingsManager settingsManager)
    {
        var setting =
            (await settingsManager.ActiveSettings)
            .FirstOrDefault(static s => s.Key == "Pulsar Host");
        BaseAddress = new(setting.Value as string ?? "http://localhost:8080");
    }
}