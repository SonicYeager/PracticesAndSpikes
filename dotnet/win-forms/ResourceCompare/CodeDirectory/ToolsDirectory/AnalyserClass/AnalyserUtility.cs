using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    static partial class Analyser
    {
        private static List<string> Scan(List<string> originalSectionNames, string[] adaptedSectionNames, string fileName)
        {
            List<string> strippedRessource = new List<string>();
            List<string> lines = System.IO.File.ReadAllLines(fileName).ToList();
            int startIndex = 0;
            int endIndex = 0;
            foreach (string sectionName in originalSectionNames)
            {
                string headerName = "// " + sectionName;
                startIndex = lines.IndexOf(headerName);
                endIndex = lines.IndexOf("/////////////////////////////////////////////////////////////////////////////", startIndex);
                if (endIndex == -1)
                {
                    endIndex = lines.Count - 1;
                }
                string currentSectionName = adaptedSectionNames[originalSectionNames.IndexOf(sectionName)];
                for (int i = startIndex; i < endIndex; i++)
                    strippedRessource.Add(currentSectionName + "/" + lines[i].Trim());
            }
            return strippedRessource;
        }

        private static List<string> ScanCompletely(string fileName)
        {
            List<string> strippedRessource = System.IO.File.ReadAllLines(fileName).ToList();
            return strippedRessource;
        }
    }
}
