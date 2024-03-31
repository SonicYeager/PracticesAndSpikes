namespace Practice.Maui.Application;

public class LaunchUriBuilder
{
    public const string RedirectPath = "oauth2/redirect";

    private readonly LaunchType _type;

    public LaunchUriBuilder(LaunchType type)
    {
        _type = type;
    }

    public Uri Build()
    {
        return _type switch
        {
            LaunchType.OAuth2Redirect => new UriBuilder(AppInfo.PackageName, "")
            {
                Path = RedirectPath,
            }.Uri,
            _ => throw new NotSupportedException(),
        };
    }
}