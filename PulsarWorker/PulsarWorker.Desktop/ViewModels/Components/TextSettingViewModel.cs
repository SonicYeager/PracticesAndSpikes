using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels.Components;

public sealed class TextSettingViewModel : ViewModelBase
{
    public TextSettingViewModel(string name)
    {
        Name = name;
    }

    private string _text = string.Empty;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public string Name { get; set; }
}