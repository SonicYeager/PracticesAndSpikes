using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Practice.Maui.Application.Services;

namespace Practice.Maui.Application;

public sealed class MauiSheetConfigProviderWrapper : ISheetConfigProvider
{
    /// <inheritdoc />
    public FileDataStore FileDataStore { get; } = new(FileSystem.Current.AppDataDirectory);

    /// <inheritdoc />
    public async Task<ClientSecrets> GetClientSecrets()
    {
        var fileName = DeviceInfo.Platform == DevicePlatform.Android ? "googleapi_android.json" : "googleapi.json";
        var stream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
        var secrets = (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets;
        return secrets;
    }
}