using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PulsarWorker.Desktop.Views.Components;

public partial class MultipleChoiceSetting : UserControl
{
    public MultipleChoiceSetting()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}