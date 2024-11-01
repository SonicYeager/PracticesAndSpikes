using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Cbam.ViewModels;
using Cbam.ViewModels.QReportDetails;

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

        var rootViewModel = ParseElement(rootElement);

        return rootViewModel;
    }

    private static QReportViewModel ParseElement(XElement element)
    {
        return element switch
        {
            _ when element.Name.LocalName == "QReport" => Handle(HandleQReportElement, element),
            _ when element.Name.LocalName == "Declarant" => Handle(HandleDeclarantElement, element),
            _ => ParseDefaultElement(element),
        };
    }

    private static QReportViewModel ParseDefaultElement(XElement element)
    {
        var viewModel = new QReportViewModel
        {
            Header = element.Name.LocalName, Children = new(), Details = null,
        };

        foreach (var childElement in element.Elements())
        {
            viewModel.Children.Add(ParseElement(childElement));
        }

        return viewModel;
    }

    private static QReportDetailsViewModel HandleQReportElement(XElement element)
    {
        return new()
        {
            SubmissionDate = DateTime.Parse(GetValue(element, "SubmissionDate"), CultureInfo.InvariantCulture),
            ReportId = GetValue(element, "ReportId"),
            ReportingPeriod = GetValue(element, "ReportingPeriod"),
            Year = int.Parse(GetValue(element, "Year"), CultureInfo.InvariantCulture),
        };
    }

    private static DeclarantDetailsViewModel HandleDeclarantElement(XElement element)
    {
        return new()
        {
            IdentificationNumber = GetValue(element, "IdentificationNumber"),
            Name = GetValue(element, "Name"),
            Role = GetValue(element, "Role"),
        };
    }

    private static QReportViewModel Handle<TElement>(Func<XElement, TElement> handleElement, XElement element)
        where TElement : ViewModelBase
    {
        var details = handleElement(element);

        var viewModel = new QReportViewModel
        {
            Header = element.Name.LocalName, Children = new(), Details = details,
        };

        foreach (var childElement in element.Elements().Where(static e => e.HasElements))
        {
            viewModel.Children.Add(ParseElement(childElement));
        }

        return viewModel;
    }

    private static string GetValue(XElement element, string name)
    {
        return element.Elements().Single(e => e.Name.LocalName == name).Value;
    }
}