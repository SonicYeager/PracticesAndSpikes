using System.Web;

namespace Practice.Maui.Application;

internal static class LaunchUriHandler
{
    private static readonly TaskCompletionSource<LaunchResult> OnReturned = new();

    public static Task<LaunchResult> LaunchResult => OnReturned.Task;

    public static bool TryHandle(Uri uri)
    {
        if (uri.Scheme != AppInfo.PackageName) return false;
        var type = uri.AbsolutePath == LaunchUriBuilder.RedirectPath ? LaunchType.OAuth2Redirect : LaunchType.Unknown;
        var query = HttpUtility.ParseQueryString(uri.Query);
        OnReturned.SetResult(new(type, query));
        return true;
    }
}