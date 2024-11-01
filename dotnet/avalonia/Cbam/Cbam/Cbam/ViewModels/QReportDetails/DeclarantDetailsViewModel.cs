namespace Cbam.ViewModels.QReportDetails;

public sealed class DeclarantDetailsViewModel : ViewModelBase
{
    public required string IdentificationNumber { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
}