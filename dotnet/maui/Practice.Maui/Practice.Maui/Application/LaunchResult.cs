using System.Collections.Specialized;

namespace Practice.Maui.Application;

internal sealed class LaunchResult
{
    public LaunchResult(LaunchType type, NameValueCollection query)
    {
        Type = type;
        Query = query;
    }

    public LaunchType Type { get; }
    public NameValueCollection Query { get; }
}