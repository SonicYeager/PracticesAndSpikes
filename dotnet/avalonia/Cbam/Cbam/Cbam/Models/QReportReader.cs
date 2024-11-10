using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cbam.Models.ElementReader;
using Cbam.ViewModels;

namespace Cbam.Models;

public sealed class QReportReader
{
    private readonly Dictionary<string, IElementReader<ViewModelBase>> _elementReaders;

    public QReportReader(Dictionary<string, IElementReader<ViewModelBase>> elementReaders)
    {
        _elementReaders = elementReaders;
    }

    public QReportViewModel Read(string filePath)
    {
        var document = XDocument.Load(filePath);
        var rootElement = document.Root;

        if (rootElement == null) throw new InvalidOperationException("The XML file is empty or invalid.");

        var rootViewModel = ParseElement(rootElement);

        return rootViewModel;
    }

    private QReportViewModel ParseElement(XElement element)
    {
        return element switch
        {
            _ when element.Name.LocalName == "QReport" => Handle(_elementReaders["QReport"].Handle, element),
            _ when element.Name.LocalName == "Declarant" => Handle(_elementReaders["Declarant"].Handle, element),
            _ when element.Name.LocalName == "ActorAddress" => Handle(_elementReaders["ActorAddress"].Handle, element),
            _ => Handle<ViewModelBase>(static _ => null, element),
        };
    }

    private QReportViewModel Handle<TElement>(Func<XElement, TElement?> handleElement, XElement element)
        where TElement : ViewModelBase
    {
        var details = handleElement(element);

        var viewModel = new QReportViewModel
        {
            Header = element.Name.LocalName, Children = [], Details = details,
        };

        foreach (var childElement in element.Elements().Where(static e => e.HasElements))
        {
            viewModel.Children.Add(ParseElement(childElement));
        }

        return viewModel;
    }
}