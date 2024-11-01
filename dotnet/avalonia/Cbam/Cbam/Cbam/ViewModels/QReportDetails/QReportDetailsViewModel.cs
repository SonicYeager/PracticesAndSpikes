using System;

namespace Cbam.ViewModels.QReportDetails;

public sealed class QReportDetailsViewModel : ViewModelBase
{
    public required DateTime SubmissionDate { get; set; }
    public required string ReportId { get; set; }
    public required string ReportingPeriod { get; set; }
    public required int Year { get; set; }
}