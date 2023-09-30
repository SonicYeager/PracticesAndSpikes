using CommunityToolkit.Mvvm.Input;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel
{
    private int _count = 0;
    public string ClickedButtonText
    {
        get => _count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";
    }

    public OverviewViewModel()
    {
        ButtonClicked = new AsyncRelayCommand(Clicked);
    }

    public IAsyncRelayCommand ButtonClicked { get; }

    private async Task Clicked()
    {
        _count++;
    }
}