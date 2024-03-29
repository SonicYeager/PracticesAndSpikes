using IFileSystem = Practice.Maui.Application.Services.IFileSystem;

namespace Practice.Maui.Application;

public class MauiFileSystemWrapper : IFileSystem
{

    /// <inheritdoc />
    public async Task<Stream> OpenReadAsync(string fileName)
    {
        return await FileSystem.Current.OpenAppPackageFileAsync(fileName);
    }

    /// <inheritdoc />
    public string AppDataDirectory
    {
        get => FileSystem.Current.AppDataDirectory;
    }
}