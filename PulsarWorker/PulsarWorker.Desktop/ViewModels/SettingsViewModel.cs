using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Desktop.Models;
using PulsarWorker.Desktop.Services;

namespace PulsarWorker.Desktop.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly SettingsModel _model;
    private readonly IServiceProvider _provider;
    private readonly UserManager _userManager;
    private IManagedNotificationManager? ManagedNotificationManager { get; set; }

    public SettingsViewModel(SettingsModel model, IServiceProvider provider, UserManager userManager)
    {
        _model = model;
        _provider = provider;
        _userManager = userManager;
    }

    public async Task LoadAsync()
    {
        await _model.GetPersistedSettings(PersistedOptions, async () => await Notify(), _userManager.CurrentUserId); // userId 1 is the test user predefined by the database project
    }


    private async Task Notify()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            ManagedNotificationManager ??= _provider.GetRequiredService<IManagedNotificationManager>();
            ManagedNotificationManager?.Show(new Notification("Settings Saved", "", NotificationType.Success));
        }).GetTask();
    }

    public ObservableCollection<object> PersistedOptions { get; init; } = new();
}