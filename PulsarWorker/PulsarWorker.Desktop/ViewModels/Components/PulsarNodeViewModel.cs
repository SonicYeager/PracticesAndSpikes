using System.Collections.ObjectModel;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels.Components;

public class PulsarNodeViewModel : ViewModelBase
{
    public PulsarNodeViewModel(ObservableCollection<PulsarNodeViewModel> subNodes, string name, IReactiveCommand deleteCommand,
        IReactiveCommand addCommand)
    {
        SubNodes = subNodes;
        Name = name;
        DeleteCommand = deleteCommand;
        AddCommand = addCommand;
    }

    public ObservableCollection<PulsarNodeViewModel> SubNodes { get; init; }
    public string Name { get; init; }
    public IReactiveCommand DeleteCommand { get; init; }
    public IReactiveCommand AddCommand { get; set; }
}