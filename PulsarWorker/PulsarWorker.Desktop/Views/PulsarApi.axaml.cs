using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PulsarWorker.Desktop.Views;

public sealed partial class PulsarApi : UserControl
{
    public PulsarApi()
    {
        AvaloniaXamlLoader.Load(this);
    }
}