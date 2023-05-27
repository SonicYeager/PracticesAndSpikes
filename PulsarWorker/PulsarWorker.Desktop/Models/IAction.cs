using System.Windows.Input;

namespace PulsarWorker.Desktop.Models;

public interface IAction
{
    public string? Header { get; set; }
    public ICommand? Command { get; set; }
}