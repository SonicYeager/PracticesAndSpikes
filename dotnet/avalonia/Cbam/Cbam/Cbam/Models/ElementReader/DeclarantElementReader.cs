using System.Xml.Linq;
using Cbam.ViewModels.QReportDetails;

namespace Cbam.Models.ElementReader;

public static class DeclarantElementReader
{
    public static DeclarantDetailsViewModel Handle(XElement element)
    {
        return new()
        {
            IdentificationNumber = element.GetValue("IdentificationNumber"),
            Name = element.GetValue("Name"),
            Role = element.GetValue("Role"),
        };
    }
}