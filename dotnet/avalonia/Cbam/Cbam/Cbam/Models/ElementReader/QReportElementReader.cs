using System;
using System.Globalization;
using System.Xml.Linq;
using Cbam.ViewModels.QReportDetails;

namespace Cbam.Models.ElementReader;

public sealed class QReportElementReader : IElementReader<QReportDetailsViewModel>
{
    public QReportDetailsViewModel Handle(XElement element)
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