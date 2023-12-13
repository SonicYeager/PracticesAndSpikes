using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Practice.Maui.ViewModels;

public sealed class OverviewViewModel : ObservableObject
{
    public OverviewViewModel()
    {
        Apogs =
        [
            new()
            {
                Title = "Test Image", Url = new("https://http.cat/images/404.jpg"), HdUrl = new("https://http.cat/images/404.jpg"),
            },
        ];
        SelectApogCommand = new AsyncRelayCommand(SelectApog);
    }
    public ObservableCollection<ApodViewModel> Apogs { get; }
    public ICommand SelectApogCommand { get; }

    private Task SelectApog()
    {
        throw new NotSupportedException();
    }
}