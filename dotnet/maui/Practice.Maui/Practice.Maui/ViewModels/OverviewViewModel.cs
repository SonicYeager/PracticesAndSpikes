using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject
{
    private int _count;
    public string ClickedButtonText
    {
        get => _count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";
    }

    public OverviewViewModel()
    {
        ButtonClicked = new AsyncRelayCommand(Clicked);
    }

    public IAsyncRelayCommand ButtonClicked { get; }

    private Task Clicked()
    {
        _count++;
        OnPropertyChanged(nameof(ClickedButtonText));
        return Task.CompletedTask;
    }
}