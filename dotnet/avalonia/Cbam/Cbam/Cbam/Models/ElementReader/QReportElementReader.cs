using System;
using System.Globalization;
using System.Xml.Linq;
using Cbam.ViewModels.QReportDetails;

namespace Cbam.Models.ElementReader;

public static class QReportElementReader
{
    public static QReportDetailsViewModel HandleQReportElement(XElement element)
    {
        return new()
        {
            SubmissionDate = DateTime.Parse(element.GetValue("SubmissionDate"), CultureInfo.InvariantCulture),
            ReportId = element.GetValue("ReportId"),
            ReportingPeriod = element.GetValue("ReportingPeriod"),
            Year = int.Parse(element.GetValue("Year"), CultureInfo.InvariantCulture),
        };
    }
}