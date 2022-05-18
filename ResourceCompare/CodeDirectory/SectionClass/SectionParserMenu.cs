using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    class SectionParserMenu
    {
        public static Tuple<List<string>, List<string>> ApplyToolsStandart(List<string> rcA, List<string> rcB)
        {
            Cleaner.StandartCleaning.CleaningMenu(rcA);
            Cleaner.StandartCleaning.CleaningMenu(rcB);
            Extractor.DoExtractorStandart.ExtractingMenu(rcA, rcB);
            Composer.DoComposerStandart(rcA, rcB);
            List<string> comparedStringsA = Comparator.DoComparatorStandart(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoComparatorStandart(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA, rcB);
        }
        public static Tuple<List<string>, List<string>> ApllyToolsForFormatSpecifier(List<string> rcA, List<string> rcB)
        {
            Cleaner.FormatSpecifierCleaning.CleaningMenu(rcA);
            Cleaner.FormatSpecifierCleaning.CleaningMenu(rcB);
            List<string> fSRCA = Extractor.DoExtractorFormatSpecifier(rcA);
            List<string> fSRCB = Extractor.DoExtractorFormatSpecifier(rcB);
            Extractor.DoExtractorStandart.ExtractingMenu(rcA, rcB);
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
            Cleaner.StringCleaning.CleaningMenu(rcA);
            Cleaner.StringCleaning.CleaningMenu(rcB);
            List<string> extractedStringsA = Extractor.DoExtractingStrings(rcA);
            List<string> extractedStringsB = Extractor.DoExtractingStrings(rcB);
            Extractor.DoExtractorStandart.ExtractingMenu(rcA, rcB);
            Composer.DoClippContentTogether(rcA, rcB, extractedStringsA, extractedStringsB);
            List<string> comparedStringsA = Comparator.DoCompareInverted(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoCompareInverted(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA, rcB);
        }

        public static List<string> ApplyToolsForSortedRC(List<string> rcA, List<string> rcB)
        {
            List<List<string>> ChoppedSectionA = Chopper.MenuChopper(rcA); 
            List<List<string>> ChoppedSectionB = Chopper.MenuChopper(rcB); 
            List<List<string>> sortedSection =  Sorter.SortSection(ChoppedSectionA, ChoppedSectionB);
            rcB = Composer.MergeMenusToSection(sortedSection, rcB); 

            return rcB;
        }
    }
}
