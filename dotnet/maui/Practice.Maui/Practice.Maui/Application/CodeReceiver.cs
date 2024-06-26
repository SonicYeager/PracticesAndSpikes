﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;

namespace Practice.Maui.Application;

public sealed class CodeReceiver : ICodeReceiver
{
    /// <summary>
    ///    Gets the redirect URI where the response will be sent.
    /// </summary>
    public string RedirectUri
    {
        get => new LaunchUriBuilder(LaunchType.OAuth2Redirect).Build().AbsoluteUri;
    }

    /// <summary>
    ///    Receives the authorization code.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="taskCancellationToken"></param>
    /// <returns></returns>
    public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url,
        CancellationToken taskCancellationToken)
    {
        var uri = url.Build().AbsoluteUri;
        await Launcher.Default.OpenAsync(uri);
        var result = await LaunchUriHandler.LaunchResult;
        if (result.Type != LaunchType.OAuth2Redirect) throw new OperationCanceledException();
        return new()
        {
            Code = result.Query.Get("code"),
            State = result.Query.Get("state"),
            Error = result.Query.Get("error"),
            ErrorDescription = result.Query.Get("error_description"),
            ErrorUri = result.Query.Get("error_uri")
        };
    }
}