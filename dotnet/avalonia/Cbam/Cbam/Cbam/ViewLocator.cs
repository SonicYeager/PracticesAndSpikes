using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Cbam.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Cbam;

public sealed class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type is not null) return (Control)_serviceProvider.GetRequiredService(type);

        return new TextBlock
        {
            Text = "Not Found: " + name,
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}