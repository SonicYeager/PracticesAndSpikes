using System.Xml.Linq;
using Cbam.ViewModels.QReportDetails;

namespace Cbam.Models.ElementReader;

public sealed class DeclarantElementReader : IElementReader<DeclarantDetailsViewModel>
{
    public DeclarantDetailsViewModel Handle(XElement element)
    {
        return new()
        {
            IdentificationNumber = element.GetValue("IdentificationNumber"),
            Name = element.GetValue("Name"),
            Role = element.GetValue("Role"),
        };
    }
}