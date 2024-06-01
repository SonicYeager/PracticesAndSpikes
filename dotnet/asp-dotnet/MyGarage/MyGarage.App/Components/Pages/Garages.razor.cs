using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyGarage.App.Application;

namespace MyGarage.App.Components.Pages;

public partial class Garages
{
    [Inject] private IMyGarageService MyGarageClient { get; init; } = null!;

    private readonly List<IGetGarages_Garages_Edges_Node> _garages = [];
    private IAsyncEnumerable<IGetGarages_Garages_Edges_Node>? _garagesStream;
    private readonly CancellationTokenSource _cts = new();
    private int _fetched = 0;

    protected override async Task OnInitializedAsync()
    {
        _garagesStream = MyGarageClient.GetGarages(_cts.Token);
        await LoadNextChunk();
    }

    [JSInvokable]
    public async Task LoadNextChunk()
    {
        if (_garagesStream != null)
            await foreach (var garage in _garagesStream.Skip(_fetched).Take(10))
            {
                _garages.Add(garage);
                _fetched++;
            }
    }

    public void Dispose()
    {
        _cts.Cancel();
    }
}