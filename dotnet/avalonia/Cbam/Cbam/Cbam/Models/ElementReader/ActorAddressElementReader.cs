using System.Xml.Linq;
using Cbam.ViewModels.QReportDetails;

namespace Cbam.Models.ElementReader;

public sealed class ActorAddressElementReader : IElementReader<ActorAddressDetailsViewModel>
{
    public ActorAddressDetailsViewModel Handle(XElement element)
    {
        return new()
        {
            Country = element.GetValue("Country"),
            SubDivision = element.GetValue("SubDivision"),
            City = element.GetValue("City"),
            Street = element.GetValue("Street"),
            StreetAdditionalLine = element.GetValue("StreetAdditionalLine"),
            Number = element.GetValue("Number"),
            Postcode = element.GetValue("Postcode"),
            POBox = element.GetValue("POBox"),
        };
    }
}