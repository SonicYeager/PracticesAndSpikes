using System.Xml.Linq;
using Cbam.ViewModels;

namespace Cbam.Models.ElementReader;

public interface IElementReader<out TElement> where TElement : ViewModelBase
{
    public TElement Handle(XElement element);
}