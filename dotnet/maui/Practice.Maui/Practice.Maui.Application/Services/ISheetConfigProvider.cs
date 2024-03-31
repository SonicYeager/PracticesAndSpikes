using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace Practice.Maui.Application.Services;

/// <summary>
///     Interface for file system operations.
/// </summary>
public interface ISheetConfigProvider
{
    public FileDataStore FileDataStore { get; }
    public Task<ClientSecrets> GetClientSecrets();
}