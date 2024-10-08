using System;
using System.Collections.Generic;
using ResourceCompare.CodeDirectory.ToolsDirectory.ChopperClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.ExtractorClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.SorterClass;

namespace ResourceCompare.CodeDirectory.SectionClass;

internal static class SectionParserStringTable
{

    public static Tuple<List<string>, List<string>> ApplyToolsStandard(List<string> rcA, List<string> rcB)
    {
        ToolsDirectory.CleanerClass.Cleaner.StandardCleaning.CleaningStringTable(rcA);
        ToolsDirectory.CleanerClass.Cleaner.StandardCleaning.CleaningStringTable(rcB);
        Extractor.DoExtractorStandart.ExtractingStringTable(rcA, rcB);
        var comparedStringsA = ToolsDirectory.ComparatorClass.Comparator.DoComparatorStandard(rcA, rcB);
        var comparedStringsB = ToolsDirectory.ComparatorClass.Comparator.DoComparatorStandard(rcB, rcA);
        rcA = comparedStringsA;
        rcB = comparedStringsB;

        return Tuple.Create(rcA, rcB);
    }

    public static Tuple<List<string>, List<string>> ApplyToolsForFormatSpecifier(List<string> rcA, List<string> rcB)
    {
        ToolsDirectory.ComposerClass.Composer.DoFormatSpecifierFixLines(rcA, rcB);
        ToolsDirectory.CleanerClass.Cleaner.FormatSpecifierCleaning.CleaningStringTable(rcA);
        ToolsDirectory.CleanerClass.Cleaner.FormatSpecifierCleaning.CleaningStringTable(rcB);
        var fSRCA = Extractor.DoExtractorFormatSpecifier(rcA);
        var fSRCB = Extractor.DoExtractorFormatSpecifier(rcB);
        Extractor.DoExtractorStandart.ExtractingStringTable(rcA, rcB);
        ToolsDirectory.ComposerClass.Composer.DoClippContentTogether(rcA, rcB, fSRCA, fSRCB);
        ToolsDirectory.CleanerClass.Cleaner.DoCleanerSpecialisedForFormatSpecifier(rcA, rcB);
        var comparedStringsA = ToolsDirectory.ComparatorClass.Comparator.DoComparatorStandard(rcA, rcB);
        var comparedStringsB = ToolsDirectory.ComparatorClass.Comparator.DoComparatorStandard(rcB, rcA);
        rcA = comparedStringsA;
        rcB = comparedStringsB;

        return Tuple.Create(rcA, rcB);
    }

    public static Tuple<List<string>, List<string>> ApplyToolsForUntranslatedStrings(List<string> rcA, List<string> rcB)
    {
        ToolsDirectory.ComposerClass.Composer.DoComposerFixStringLine(rcA, rcB);
        ToolsDirectory.CleanerClass.Cleaner.StringCleaning.CleaningStringTable(rcA);
        ToolsDirectory.CleanerClass.Cleaner.StringCleaning.CleaningStringTable(rcB);
        var extractedStringsA = Extractor.DoExtractingStrings(rcA);
        var extractedStringsB = Extractor.DoExtractingStrings(rcB);
        Extractor.DoExtractorStandart.ExtractingStringTable(rcA, rcB);
        ToolsDirectory.ComposerClass.Composer.DoClippContentTogether(rcA, rcB, extractedStringsA, extractedStringsB);
        var comparedStringsA = ToolsDirectory.ComparatorClass.Comparator.DoCompareInverted(rcA, rcB);
        var comparedStringsB = ToolsDirectory.ComparatorClass.Comparator.DoCompareInverted(rcB, rcA);
        rcA = comparedStringsA;
        rcB = comparedStringsB;

        return Tuple.Create(rcA, rcB);
    }

    public static List<string> ApplyToolsForSortedRC(List<string> rcA, List<string> rcB)
    {
        var choppedSectionA = Chopper.StringTableChopper(rcA);
        rcB = Sorter.SortSectionStringTable(choppedSectionA, rcB);

        return rcB;
    }
}