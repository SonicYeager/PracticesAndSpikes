using System;
using System.Collections.ObjectModel;
using PulsarWorker.Desktop.Models;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels;

//TODO just enable to add/delete a topic or namespace
//and observe any given topics in any given namespace as a tree view with live view
public class PulsarApiViewModel : ViewModelBase
{
    public PulsarApiViewModel(IPulsarNode rootPulsarNode)
    {
        Nodes.Add(rootPulsarNode);
        
        this.WhenAnyValue(x => x.Nodes.Count)
            .Subscribe(x =>
            {
                if(x > 1) throw new InvalidOperationException("Cannot have more than one root node!");
            });
    }

    public ObservableCollection<IPulsarNode> Nodes { get; init; } = new();

}