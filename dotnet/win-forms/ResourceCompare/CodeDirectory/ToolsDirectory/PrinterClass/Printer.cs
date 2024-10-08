using System;
using System.Collections.Generic;
using System.IO;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.PrinterClass;

public static partial class Printer
{
    public static void PrintStandart(List<List<string>> notRdyToPrint, string newDestination, string fileNameTop, string fileNameBottom)
    {
        using (File.Create(@newDestination))
        {
        }

        var deco = new string[]
        {
            "-------------------------------------------------------------------------------------------------------[",
            "]-------------------------------------------------------------------------------------------------------",
        };


        var head = string.Format("{0,-120}{1,-1}{2, -120}", fileNameTop, " ", fileNameBottom);
        using (var file = new StreamWriter(@newDestination, false))
        {
            file.WriteLine(head);

            file.WriteLine(deco[0] + " String Table " + deco[1]);
            Print(notRdyToPrint, 0, 3, @newDestination, file);

            file.WriteLine(deco[0] + " Dialog " + deco[1]);
            Print(notRdyToPrint, 1, 4, @newDestination, file);

            file.WriteLine(deco[0] + " Menu " + deco[1]);
            Print(notRdyToPrint, 2, 5, @newDestination, file);
        }
    }

    public static void PrintFormatSpecifier(List<List<string>> notRdyToPrint, string newDestination, string fileNameTop,
        string fileNameBottom)
    {
        using (File.Create(@newDestination))
        {
        }

        var deco = new string[]
        {
            "-------------------------------------------------------------------------------------------------------[",
            "]-------------------------------------------------------------------------------------------------------",
        };

        var head = string.Format("{0,-120}{1,-1}{2, -120}", fileNameTop, " ", fileNameBottom);
        using (var file = new StreamWriter(@newDestination, false))
        {
            file.WriteLine(head);

            file.WriteLine(deco[0] + " String Table " + deco[1]);
            PrintsOnOneLine(notRdyToPrint, 0, 3, @newDestination, file);

            file.WriteLine(deco[0] + " Dialog " + deco[1]);
            PrintsOnOneLine(notRdyToPrint, 1, 4, @newDestination, file);

            file.WriteLine(deco[0] + " Menu " + deco[1]);
            PrintsOnOneLine(notRdyToPrint, 2, 5, @newDestination, file);
        }
    }

    public static void PrintUntranslatedStrings(List<List<string>> notRdyToPrint, string newDestination, string fileNameTop,
        string fileNameBottom)
    {
        using (File.Create(@newDestination))
        {
        }


        var deco = new string[]
        {
            "-------------------------------------------------------------------------------------------------------[",
            "]-------------------------------------------------------------------------------------------------------",
        };


        var head = string.Format("{0,-120}{1,-1}{2, -120}", fileNameTop, " ", fileNameBottom);
        using (var file = new StreamWriter(@newDestination, false))
        {
            file.WriteLine(head);

            file.WriteLine(deco[0] + " String Table " + deco[1]);
            Print(notRdyToPrint, 0, @newDestination, file);

            file.WriteLine(deco[0] + " Dialog " + deco[1]);
            Print(notRdyToPrint, 1, @newDestination, file);

            file.WriteLine(deco[0] + " Menu " + deco[1]);
            Print(notRdyToPrint, 2, @newDestination, file);
        }
    }

    public static void PrintNewRC(List<string> newRC, string newDestination)
    {
        using (File.Create(@newDestination))
        {
        }

        File.WriteAllLines(newDestination, newRC);
    }
}