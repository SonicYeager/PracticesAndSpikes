using Avalonia.Controls;
using Avalonia.Input;
using System.Collections.Generic;

namespace Prototype
{
    public static class WindowHelperFunctions
    {
        public static T FindUserControl<T>(IEnumerable<Avalonia.LogicalTree.ILogical> list, string name) where T : UserControl
        {
            T Recursion(IEnumerable<Avalonia.LogicalTree.ILogical> list)
            {
                foreach (Avalonia.LogicalTree.ILogical i in list)
                {
                    if (i is Avalonia.INamed named && named is T ret && ret.Name == name)
                    {
                        return ret;
                    }
                    else
                    {
                        T r = Recursion(i.LogicalChildren);
                        if (r != null)
                        {
                            return r;
                        }
                    }
                }
                return null;
            }
            return Recursion(list);
        }
    }

}
