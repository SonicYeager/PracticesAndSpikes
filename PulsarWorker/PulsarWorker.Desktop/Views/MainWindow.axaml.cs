using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PulsarWorker.Desktop.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}