using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Calculusius.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Calculusius;

public class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _serviceProvider;
    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)_serviceProvider.GetRequiredService(type);
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}