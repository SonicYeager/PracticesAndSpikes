using ReactiveUI;

namespace Cbam.ViewModels;

public class ViewModelBase : ReactiveObject, IActivatableViewModel
{
    /// <inheritdoc />
    public ViewModelActivator Activator { get; }

    public ViewModelBase()
    {
        Activator = new();
    }
}