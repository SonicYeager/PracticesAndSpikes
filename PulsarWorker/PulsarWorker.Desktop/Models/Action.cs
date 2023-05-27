using System.Windows.Input;

namespace PulsarWorker.Desktop.Models;

public sealed class Action : IAction
{
    public Action(string header, ICommand? command)
    {
        Header = header;
        Command = command;
    }
    
    public string? Header { get; set; }
    public ICommand? Command { get; set; }
}