using System.IO;
using System.Threading.Tasks;

namespace Practice.Maui.Application.Services;

/// <summary>
///     Interface for file system operations.
/// </summary>
public interface IFileSystem
{

    /// <summary>
    ///     Gets the application data directory.
    /// </summary>
    public string AppDataDirectory { get; }
    /// <summary>
    ///     Opens a file for reading.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public Task<Stream> OpenReadAsync(string fileName);
}