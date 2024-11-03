using System.Xml.Linq;
using Cbam.ViewModels.QReportDetails;

namespace Cbam.Models.ElementReader;

public static class DeclarantElementReader
{
    public static DeclarantDetailsViewModel HandleDeclarantElement(XElement element)
    {
        return new()
        {
            IdentificationNumber = element.GetValue("IdentificationNumber"),
            Name = element.GetValue("Name"),
            Role = element.GetValue("Role"),
        };
    }
}