using PdfSharp.Fonts;
using PdfToolKit.Library;

Console.WriteLine("Setting global font resolver...");
GlobalFontSettings.FontResolver = new FailsafeFontResolver();

Console.WriteLine("Generating dummy PDFs for merge example...");
Examples.CreateDummyPdf("doc1.pdf", "This is document 1.");
Examples.CreateDummyPdf("doc2.pdf", "This is document 2.");

Console.WriteLine("Running PDF Toolkit Examples...");
Examples.DemoAll();
Console.WriteLine("PDF Toolkit Examples finished.");
