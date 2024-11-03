namespace Cbam.ViewModels.QReportDetails;

public sealed class ActorAddressDetailsViewModel : ViewModelBase
{
    public required string Country { get; set; }
    public required string SubDivision { get; set; }
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string StreetAdditionalLine { get; set; }
    public required string Number { get; set; }
    public required string Postcode { get; set; }
    public required string POBox { get; set; }
}