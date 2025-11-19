using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SkiaSharp;
using Document = MigraDoc.DocumentObjectModel.Document;

namespace PdfToolKit.Library;

/// <summary>
/// 100% MIT-lizenziertes Hybrid-Toolkit
/// - MigraDoc/PDFSharp für PDF-Struktur (MIT)
/// - SkiaSharp für komplexe Grafiken (MIT)
/// </summary>
public sealed class PdfToolkit
{
    // === PDF MERGING (MigraDoc) ===

    public static void MergePdfs(string[] inputFiles, string outputPath)
    {
        using var outputDocument = new PdfDocument();

        foreach (var file in inputFiles)
        {
            using var inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
            foreach (var page in inputDocument.Pages)
            {
                outputDocument.AddPage(page);
            }
        }

        outputDocument.Save(outputPath);
    }

    // === DOKUMENT-ERSTELLUNG (MigraDoc) ===

    public static void CreateDocument(string outputPath,
        Action<Document> setupDocument)
    {
        var document = new Document();
        setupDocument(document);

        var pdfRenderer = new PdfDocumentRenderer
        {
            Document = document,
        };

        pdfRenderer.RenderDocument();
        pdfRenderer.PdfDocument.Save(outputPath);
    }

    // === TEXT & STRUKTUR (MigraDoc) ===

    public static Document CreateBasicDocument(string title)
    {
        var document = new Document();

        // Styles
        var style = document.Styles["Normal"];
        style.Font.Name = "Arial";
        style.Font.Size = 12;

        // Section
        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;

        // Titel
        var paragraph = section.AddParagraph(title);
        paragraph.Format.Font.Size = 24;
        paragraph.Format.Font.Bold = true;
        paragraph.Format.SpaceAfter = 12;

        return document;
    }

    public static void AddTable(Section section, string[][] data)
    {
        var table = section.AddTable();
        table.Borders.Width = 0.5;

        // Spalten
        var cols = data[0].Length;
        for (var i = 0; i < cols; i++)
        {
            table.AddColumn(Unit.FromCentimeter(5));
        }

        // Header
        var headerRow = table.AddRow();
        headerRow.HeadingFormat = true;
        headerRow.Format.Font.Bold = true;
        for (var j = 0; j < cols; j++)
        {
            headerRow.Cells[j].AddParagraph(data[0][j]);
        }

        // Daten
        for (var i = 1; i < data.Length; i++)
        {
            var row = table.AddRow();
            for (var j = 0; j < cols; j++)
            {
                row.Cells[j].AddParagraph(data[i][j]);
            }
        }
    }

    // === FORMULAR-FELDER SIMULIEREN (MigraDoc) ===

    public static void AddFormFields(Section section,
        Dictionary<string, string> fields)
    {
        var table = section.AddTable();
        table.Borders.Width = 0;
        table.AddColumn(Unit.FromCentimeter(5));
        table.AddColumn(Unit.FromCentimeter(10));

        foreach (var field in fields)
        {
            var row = table.AddRow();

            // Label
            var labelCell = row.Cells[0];
            labelCell.AddParagraph(field.Key);
            labelCell.Format.Font.Bold = true;

            // Value mit Border
            var valueCell = row.Cells[1];
            valueCell.Borders.Bottom.Width = 0.5;
            valueCell.AddParagraph(field.Value);
        }
    }

    // === BILDER (MigraDoc) ===

    public static void AddImage(Section section, string imagePath,
        double widthCm = 10)
    {
        var image = section.AddImage(imagePath);
        image.Width = Unit.FromCentimeter(widthCm);
        image.LockAspectRatio = true;
    }

    // === KOMPLEXE GRAFIKEN (SkiaSharp → einbetten) ===

    public void AddSkiaGraphic(Section section,
        Action<SKCanvas> drawAction,
        int width = 400, int height = 300)
    {
        // SkiaSharp Grafik als PNG rendern
        var imageInfo = new SKImageInfo(width, height);
        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        drawAction(canvas);

        // Als temporäres Bild speichern
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        var tempPath = Path.GetTempFileName() + ".png";
        using (var fileStream = File.OpenWrite(tempPath))
        {
            data.SaveTo(fileStream);
        }

        // In Dokument einbetten
        AddImage(section, tempPath, width / 28.35); // Pixel → cm

        // Temp-Datei löschen
        File.Delete(tempPath);
    }

    // === ECHTE PDF-FORMULAR-FELDER (PDFSharp Low-Level) ===

    public static void CreateInteractivePdfForm(string outputPath,
        Dictionary<string, string> fields)
    {
        using var document = new PdfDocument();
        document.AddPage();

        // Hinweis: Echte AcroForm-Felder erfordern low-level PDFSharp
        // Dies ist eine vereinfachte Version

        // TODO: PdfAcroForm implementation
        // Dies würde native PDF-Formularfelder erstellen
        // Momentan nur statischer Text möglich

        document.Save(outputPath);

        throw new NotImplementedException(
            "Interaktive PDF-Formulare erfordern tiefere PDFSharp-Integration. " +
            "Nutze stattdessen gerenderte Felder mit MigraDoc.");
    }
}

// === BEISPIELE ===

public class Examples
{
    public static void CreateDummyPdf(string outputPath, string title)
    {
        var toolkit = new PdfToolkit();
        PdfToolkit.CreateDocument(outputPath, doc =>
        {
            var section = doc.AddSection();
            var paragraph = section.AddParagraph(title);
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Font.Bold = true;
        });
    }

    public static void DemoAll()
    {
        var toolkit = new PdfToolkit();

        // 1. PDFs mergen
        PdfToolkit.MergePdfs(
            [
                "doc1.pdf", "doc2.pdf",
            ],
            "merged.pdf"
        );

        // 2. Dokument mit Text und Tabelle
        PdfToolkit.CreateDocument("report.pdf", static doc =>
        {
            var section = doc.AddSection();
            section.AddParagraph("Verkaufsbericht Q4 2024");

            var data = new string[][]
            {
                [
                    "Produkt", "Verkäufe", "Umsatz",
                ],
                [
                    "Widget A", "150", "1.500€",
                ],
                [
                    "Widget B", "200", "4.000€",
                ],
                [
                    "Widget C", "100", "1.500€",
                ],
            };

            PdfToolkit.AddTable(section, data);
        });

        // 3. Formular-ähnliches Dokument
        PdfToolkit.CreateDocument("form.pdf", static doc =>
        {
            var section = doc.AddSection();
            section.AddParagraph("Antrag").Format.Font.Size = 20;

            var fields = new Dictionary<string, string>
            {
                ["Name"] = "Max Mustermann",
                ["Geburtsdatum"] = "01.01.1990",
                ["Adresse"] = "Musterstraße 123, 12345 Musterstadt",
                ["E-Mail"] = "max@example.com",
            };

            PdfToolkit.AddFormFields(section, fields);
        });

        // 4. Dokument mit SkiaSharp-Grafik
        PdfToolkit.CreateDocument("with-chart.pdf", doc =>
        {
            var section = doc.AddSection();
            section.AddParagraph("Statistik-Bericht").Format.Font.Size = 20;

            // SkiaSharp Custom Chart
            toolkit.AddSkiaGraphic(section, static canvas =>
            {
                // Hintergrund
                canvas.Clear(SKColors.White);

                // Balkendiagramm zeichnen
                var paint = new SKPaint
                {
                    Color = SKColors.Blue, Style = SKPaintStyle.Fill,
                };

                int[] values =
                [
                    50, 80, 120, 90,
                ];
                for (var i = 0; i < values.Length; i++)
                {
                    canvas.DrawRect(
                        50 + i * 80,
                        300 - values[i],
                        60,
                        values[i],
                        paint
                    );
                }

                // Achsen
                using var axisPaint = new SKPaint
                {
                    Color = SKColors.Black, StrokeWidth = 2, Style = SKPaintStyle.Stroke,
                };

                canvas.DrawLine(40, 300, 400, 300, axisPaint); // X
                canvas.DrawLine(40, 50, 40, 300, axisPaint); // Y
            }, width: 400, height: 300);
        });

        // 5. Komplexes mehrseitiges Dokument
        PdfToolkit.CreateDocument("complex.pdf", static doc =>
        {
            // Seite 1: Deckblatt
            var section1 = doc.AddSection();
            var title = section1.AddParagraph("Jahresbericht 2024");
            title.Format.Font.Size = 32;
            title.Format.SpaceAfter = 100;
            title.Format.Alignment = ParagraphAlignment.Center;

            section1.AddParagraph("Erstellt am: " + DateTime.Now.ToString("dd.MM.yyyy"));

            // Seite 2: Inhalt mit Bild
            var section2 = doc.AddSection();
            section2.AddPageBreak();
            section2.AddParagraph("Zusammenfassung").Format.Font.Size = 20;
            section2.AddParagraph(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
            );

            // Optional: Bild hinzufügen
            // toolkit.AddImage(section2, "logo.png", 5);

            // Seite 3: Tabelle
            var section3 = doc.AddSection();
            section3.AddPageBreak();
            section3.AddParagraph("Finanzdaten").Format.Font.Size = 20;

            var financialData = new string[][]
            {
                [
                    "Quartal", "Umsatz", "Gewinn", "Wachstum",
                ],
                [
                    "Q1", "100.000€", "15.000€", "+5%",
                ],
                [
                    "Q2", "120.000€", "18.000€", "+20%",
                ],
                [
                    "Q3", "115.000€", "17.000€", "-4%",
                ],
                [
                    "Q4", "140.000€", "22.000€", "+22%",
                ],
            };

            PdfToolkit.AddTable(section3, financialData);
        });
    }
}