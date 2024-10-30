using System;
using System.Xml.Linq;
using Cbam.ViewModels;

namespace Cbam.Models;

public static class QReportReader
{
    public static QReportViewModel Read(string filePath)
    {
        var document = XDocument.Load(filePath);
        var rootElement = document.Root;

        if (rootElement == null)
        {
            throw new InvalidOperationException("The XML file is empty or invalid.");
        }

        var rootViewModel = new QReportViewModel
        {
            Header = rootElement.Name.LocalName,
            Children = new(),
        };

        foreach (var element in rootElement.Elements())
        {
            rootViewModel.Children.Add(ParseElement(element));
        }

        return rootViewModel;
    }

    private static QReportViewModel ParseElement(XElement element)
    {
        var viewModel = new QReportViewModel
        {
            Header = element.Name.LocalName,
            Children = new(),
        };

        foreach (var childElement in element.Elements())
        {
            viewModel.Children.Add(ParseElement(childElement));
        }

        return viewModel;
    }
}