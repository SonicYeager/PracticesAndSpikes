using System;
using System.Globalization;
using System.Linq;
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

        var rootViewModel = ParseElement(rootElement);

        return rootViewModel;
    }

    private static QReportViewModel ParseElement(XElement element)
    {
        return element switch
        {
            _ when element.Name.LocalName == "QReport" => HandleQReportElement(element),
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

    private static QReportViewModel HandleQReportElement(XElement element)
    {
        var details = new QReportDetailsViewModel
        {
            SubmissionDate = DateTime.Parse(GetValue(element, "SubmissionDate"), CultureInfo.InvariantCulture),
            ReportId = GetValue(element, "ReportId"),
            ReportingPeriod = GetValue(element, "ReportingPeriod"),
            Year = int.Parse(GetValue(element, "Year"), CultureInfo.InvariantCulture),
        };

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