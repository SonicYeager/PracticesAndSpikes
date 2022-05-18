using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCompare
{
    class SectionParserDialog
    {
        public static Tuple<List<string>, List<string>> ApplyToolsStandart(List<string> rcA, List<string> rcB)
        {
            Cleaner.StandartCleaning.CleaningDialog(rcA);
            Cleaner.StandartCleaning.CleaningDialog(rcB);
            Extractor.DoExtractorStandart.ExtractingDialog(rcA, rcB);
            Composer.DoComposerStandart(rcA, rcB);
            List<string> comparedStringsA = Comparator.DoComparatorStandart(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoComparatorStandart(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA, rcB);
        }
        public static Tuple<List<string>, List<string>> ApllyToolsForFormatSpecifier(List<string> rcA, List<string> rcB)
        {
            Cleaner.FormatSpecifierCleaning.CleaningDialog(rcA);
            Cleaner.FormatSpecifierCleaning.CleaningDialog(rcB);
            List<string> fSRCA = Extractor.DoExtractorFormatSpecifier(rcA);
            List<string> fSRCB = Extractor.DoExtractorFormatSpecifier(rcB);
            Extractor.DoExtractorStandart.ExtractingDialog(rcA, rcB);
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
            Cleaner.StringCleaning.CleaningDialog(rcA);
            Cleaner.StringCleaning.CleaningDialog(rcB);
            List<string> extractedStringsA = Extractor.DoExtractingStrings(rcA);
            List<string> extractedStringsB = Extractor.DoExtractingStrings(rcB);
            Extractor.DoExtractorStandart.ExtractingDialog(rcA, rcB);
            Composer.DoClippContentTogether(rcA, rcB, extractedStringsA, extractedStringsB);
            List<string> comparedStringsA = Comparator.DoCompareInverted(rcA, rcB);
            List<string> comparedStringsB = Comparator.DoCompareInverted(rcB, rcA);
            rcA = comparedStringsA;
            rcB = comparedStringsB;

            return Tuple.Create(rcA, rcB);
        }

        public static List<string> ApplyToolsForSortedRC(List<string> rcA, List<string> rcB)
        {
            List<List<string>> ChoppedSectionA = Chopper.DialogChopper(rcA); 
            List<List<string>> ChoppedSectionB = Chopper.DialogChopper(rcB);
            List<List<string>> sortedSection = Sorter.SortSection(ChoppedSectionA, ChoppedSectionB);
            rcB = Composer.MergeDialogsToSection(sortedSection, rcB); 

            return rcB;
        }
    }
}
