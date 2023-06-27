using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace PulsarWorker.Desktop.ViewModels.Components;

public sealed class TextSettingViewModel : ViewModelBase
{
    private readonly Func<string, string?, Task> _onTextSettingChanged;
    public TextSettingViewModel(string name, Func<string, string?, Task> onTextSettingChanged)
    {
        Name = name;
        _onTextSettingChanged = onTextSettingChanged;
        this.WhenAnyValue(static x => x.Text)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Subscribe(async x => { await _onTextSettingChanged(Name, x); });
    }

    private string? _text = string.Empty;

    public string? Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    public string Name { get; set; }
}