using System;
using System.Collections.Generic;
using ResourceCompare.CodeDirectory.ToolsDirectory.ChopperClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.ExtractorClass;
using ResourceCompare.CodeDirectory.ToolsDirectory.SorterClass;

namespace ResourceCompare.CodeDirectory.SectionClass;

public static class SectionParserMenu
{
    public static Tuple<List<string>, List<string>> ApplyToolsStandard(List<string> rcA, List<string> rcB)
    {
        ToolsDirectory.CleanerClass.Cleaner.StandardCleaning.CleaningMenu(rcA);
        ToolsDirectory.CleanerClass.Cleaner.StandardCleaning.CleaningMenu(rcB);
        Extractor.DoExtractorStandart.ExtractingMenu(rcA, rcB);
        ToolsDirectory.ComposerClass.Composer.DoComposerStandard(rcA, rcB);
        var comparedStringsA = ToolsDirectory.ComparatorClass.Comparator.DoComparatorStandard(rcA, rcB);
        var comparedStringsB = ToolsDirectory.ComparatorClass.Comparator.DoComparatorStandard(rcB, rcA);
        rcA = comparedStringsA;
        rcB = comparedStringsB;

        return Tuple.Create(rcA, rcB);
    }
    public static Tuple<List<string>, List<string>> ApllyToolsForFormatSpecifier(List<string> rcA, List<string> rcB)
    {
        ToolsDirectory.CleanerClass.Cleaner.FormatSpecifierCleaning.CleaningMenu(rcA);
        ToolsDirectory.CleanerClass.Cleaner.FormatSpecifierCleaning.CleaningMenu(rcB);
        var fSRCA = Extractor.DoExtractorFormatSpecifier(rcA);
        var fSRCB = Extractor.DoExtractorFormatSpecifier(rcB);
        Extractor.DoExtractorStandart.ExtractingMenu(rcA, rcB);
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
        ToolsDirectory.CleanerClass.Cleaner.StringCleaning.CleaningMenu(rcA);
        ToolsDirectory.CleanerClass.Cleaner.StringCleaning.CleaningMenu(rcB);
        var extractedStringsA = Extractor.DoExtractingStrings(rcA);
        var extractedStringsB = Extractor.DoExtractingStrings(rcB);
        Extractor.DoExtractorStandart.ExtractingMenu(rcA, rcB);
        ToolsDirectory.ComposerClass.Composer.DoClippContentTogether(rcA, rcB, extractedStringsA, extractedStringsB);
        var comparedStringsA = ToolsDirectory.ComparatorClass.Comparator.DoCompareInverted(rcA, rcB);
        var comparedStringsB = ToolsDirectory.ComparatorClass.Comparator.DoCompareInverted(rcB, rcA);
        rcA = comparedStringsA;
        rcB = comparedStringsB;

        return Tuple.Create(rcA, rcB);
    }

    public static List<string> ApplyToolsForSortedRC(List<string> rcA, List<string> rcB)
    {
        var ChoppedSectionA = Chopper.MenuChopper(rcA);
        var ChoppedSectionB = Chopper.MenuChopper(rcB);
        var sortedSection = Sorter.SortSection(ChoppedSectionA, ChoppedSectionB);
        rcB = ToolsDirectory.ComposerClass.Composer.MergeMenusToSection(sortedSection, rcB);

        return rcB;
    }
}