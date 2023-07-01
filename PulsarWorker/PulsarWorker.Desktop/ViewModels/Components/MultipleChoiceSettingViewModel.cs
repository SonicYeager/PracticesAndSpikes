using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels.Components;

public sealed class MultipleChoiceSettingViewModel : ViewModelBase
{
    private readonly Func<string, string?, Task> _onChoiceChanged;
    public MultipleChoiceSettingViewModel(string name, IEnumerable<string> choices, Func<string, string?, Task> onChoiceChanged)
    {
        Name = name;
        Choices = choices;
        _onChoiceChanged = onChoiceChanged;
    }

    private string? _currentChoice = string.Empty;

    public string? CurrentChoice
    {
        get => _currentChoice;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentChoice, value);
            _onChoiceChanged(Name, value);
        }
    }

    public string Name { get; set; }

    public IEnumerable<string> Choices { get; set; }
}