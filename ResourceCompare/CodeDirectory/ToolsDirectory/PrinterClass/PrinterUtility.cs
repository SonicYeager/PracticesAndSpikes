using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    static partial class Printer
    {
        private static void Print(List<List<string>> Content, int id1, int id2, string newDestination, System.IO.StreamWriter file)
        {

            foreach (string str in Content[id1])
            {
                string line = string.Format("{0,-120}{1,-1}{2,-120}", str, " ", " ");
                file.WriteLine(line);
            }
            foreach (string str in Content[id2])
            {
                string line = string.Format("{0,-120}{1,-1}{2,-120}", " ", " ", str);
                file.WriteLine(line);
            }
        }

        private static void PrintsOnOneLine(List<List<string>> Content, int id1, int id2, string newDestination, System.IO.StreamWriter file)
        {
            List<string> RCS = new List<string>();
            List<string> RCD = new List<string>();
            List<string> RCM = new List<string>();
            List<List<string>> ContentID = new List<List<string>>() { RCS, RCD, RCM };

            List<string> seperated2 = new List<string>();



            foreach (string str in Content[id1])
            {
                string[] seperated = str.Split('|');
                ContentID[id1].Add(seperated[0]);
                seperated2.Add(seperated[1]);
            }
            Content[id1].Clear();
            foreach (string str in seperated2)
            {
                Content[id1].Add(str);
            }
            seperated2.Clear();

            int index = 0;
            foreach (string str in Content[id1])
            {
                string line = string.Format("{0,-90}{1,-40}{2,-40}", ContentID[id1][index], str, Content[id2][index]);
                file.WriteLine(line);
                index++;
            }
        }

        private static void Print(List<List<string>> Content, int id1, string newDestination, System.IO.StreamWriter file)
        {
            List<string> RCS = new List<string>();
            List<string> RCD = new List<string>();
            List<string> RCM = new List<string>();
            List<List<string>> ContentID = new List<List<string>>() { RCS, RCD, RCM };

            List<string> seperated2 = new List<string>(); ;

            foreach (string str in Content[id1])
            {
                string[] seperated = str.Split('|');
                ContentID[id1].Add(seperated[0]);
                seperated2.Add(seperated[1]);
            }
            Content[id1].Clear();
            foreach (string str in seperated2)
            {
                Content[id1].Add(str);
            }
            seperated2.Clear();

            foreach (string str in Content[id1])
            {
                string line = string.Format("{0,-90}{1,-120}", ContentID[id1][Content[id1].IndexOf(str)], str);
                file.WriteLine(line);
            }
        }
    }
}
