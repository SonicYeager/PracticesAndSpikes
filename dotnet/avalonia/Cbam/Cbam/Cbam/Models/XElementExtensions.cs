using System.Linq;
using System.Xml.Linq;

namespace Cbam.Models;

public static class XElementExtensions
{
    public static string GetValue(this XElement element, string name)
    {
        return element.Elements().Single(e => e.Name.LocalName == name).Value;
    }
}