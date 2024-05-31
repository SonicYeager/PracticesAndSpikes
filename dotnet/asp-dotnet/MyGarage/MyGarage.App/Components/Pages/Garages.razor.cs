using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyGarage.App.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyGarage.App.Components.Pages
{
    public partial class Garages
    {
        [Inject] private IMyGarageService MyGarageClient { get; init; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; init; }

        private List<IGetGarages_Garages_Edges_Node> _garages = new();
        private IAsyncEnumerable<IGetGarages_Garages_Edges_Node>? _garagesStream;
        private CancellationTokenSource _cts = new();

        protected override async Task OnInitializedAsync()
        {
            _garagesStream = MyGarageClient.GetGarages(_cts.Token);
            await LoadNextChunk();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnetHelper = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("detectScrollToBottom", dotnetHelper);
            }
        }

        [JSInvokable]
        public async Task LoadNextChunk()
        {
            if (_garagesStream != null)
            {
                await foreach (var garage in _garagesStream.Take(10))
                {
                    _garages.Add(garage);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}