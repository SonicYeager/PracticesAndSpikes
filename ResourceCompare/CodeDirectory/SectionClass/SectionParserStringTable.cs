using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceCompare;

namespace ResourceCompare
{
    static class SectionParserStringTable
    {

        public static Tuple<List<string>, List<string>> ApplyToolsStandart(List<string> rcA, List<string> rcB)
        {
            Cleaner.StandartCleaning.CleaningStringTable(rcA);
            Cleaner.StandartCleaning.CleaningStringTable(rcB);
            Extractor.DoExtractorStandart.ExtractingStringTable(rcA, rcB);
            List<string> comparedStringsA = Comparator.DoComparatorStandart(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoComparatorStandart(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA, rcB);
        }

        public static Tuple<List<string>, List<string>> ApplyToolsForFormatSpecifier(List<string> rcA, List<string> rcB)
        {
            Composer.DoFormatSpecifierFixLines(rcA, rcB);
            Cleaner.FormatSpecifierCleaning.CleaningStringTable(rcA);
            Cleaner.FormatSpecifierCleaning.CleaningStringTable(rcB);
            List<string> fSRCA = Extractor.DoExtractorFormatSpecifier(rcA);
            List<string> fSRCB = Extractor.DoExtractorFormatSpecifier(rcB);
            Extractor.DoExtractorStandart.ExtractingStringTable(rcA, rcB);
            Composer.DoClippContentTogether(rcA, rcB, fSRCA, fSRCB);
            Cleaner.DoCleanerSpecialisedForFormatSpecifier(rcA, rcB);
            List<string> comparedStringsA = Comparator.DoComparatorStandart(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoComparatorStandart(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA, rcB);
        }

        public static Tuple<List<string>, List<string>> ApplyToolsForUntranslatedStrings(List<string> rcA, List<string> rcB)
        {
            Composer.DoComposerFixStringLine(rcA, rcB);
            Cleaner.StringCleaning.CleaningStringTable(rcA);
            Cleaner.StringCleaning.CleaningStringTable(rcB);
            List<string> extractedStringsA = Extractor.DoExtractingStrings(rcA);
            List<string> extractedStringsB = Extractor.DoExtractingStrings(rcB);
            Extractor.DoExtractorStandart.ExtractingStringTable(rcA, rcB);
            Composer.DoClippContentTogether(rcA, rcB, extractedStringsA, extractedStringsB);
            List<string> comparedStringsA = Comparator.DoCompareInverted(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoCompareInverted(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA,rcB);
        }

        public static List<string> ApplyToolsForSortedRC(List<string> rcA, List<string> rcB)
        {
            List<List<string>> ChoppedSectionA = Chopper.StringTableChopper(rcA); 
            rcB = Sorter.SortSectionStringTable(ChoppedSectionA, rcB);

            return rcB;
        }
    }
}
