using System.Collections.Generic;
using System.IO;

namespace ResourceCompare.CodeDirectory.ToolsDirectory.PrinterClass;

static partial class Printer
{
    private static void Print(List<List<string>> Content, int id1, int id2, string newDestination, StreamWriter file)
    {
        foreach (var str in Content[id1])
        {
            var line = string.Format("{0,-120}{1,-1}{2,-120}", str, " ", " ");
            file.WriteLine(line);
        }

        foreach (var str in Content[id2])
        {
            var line = string.Format("{0,-120}{1,-1}{2,-120}", " ", " ", str);
            file.WriteLine(line);
        }
    }

    private static void PrintsOnOneLine(List<List<string>> Content, int id1, int id2, string newDestination, StreamWriter file)
    {
        var RCS = new List<string>();
        var RCD = new List<string>();
        var RCM = new List<string>();
        var ContentID = new List<List<string>>()
        {
            RCS, RCD, RCM,
        };

        var seperated2 = new List<string>();


        foreach (var str in Content[id1])
        {
            var seperated = str.Split('|');
            ContentID[id1].Add(seperated[0]);
            seperated2.Add(seperated[1]);
        }

        Content[id1].Clear();
        foreach (var str in seperated2)
        {
            Content[id1].Add(str);
        }

        seperated2.Clear();

        var index = 0;
        foreach (var str in Content[id1])
        {
            var line = string.Format("{0,-90}{1,-40}{2,-40}", ContentID[id1][index], str, Content[id2][index]);
            file.WriteLine(line);
            index++;
        }
    }

    private static void Print(List<List<string>> Content, int id1, string newDestination, StreamWriter file)
    {
        var RCS = new List<string>();
        var RCD = new List<string>();
        var RCM = new List<string>();
        var ContentID = new List<List<string>>()
        {
            RCS, RCD, RCM,
        };

        var seperated2 = new List<string>();
        ;

        foreach (var str in Content[id1])
        {
            var seperated = str.Split('|');
            ContentID[id1].Add(seperated[0]);
            seperated2.Add(seperated[1]);
        }

        Content[id1].Clear();
        foreach (var str in seperated2)
        {
            Content[id1].Add(str);
        }

        seperated2.Clear();

        foreach (var str in Content[id1])
        {
            var line = string.Format("{0,-90}{1,-120}", ContentID[id1][Content[id1].IndexOf(str)], str);
            file.WriteLine(line);
        }
    }
}