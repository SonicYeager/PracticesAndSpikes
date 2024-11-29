using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace LibraryPlayground.Pdf;

public sealed class PdfJuggler
{
    public void Generate()
    {
        var file = new PdfDocument();

        var page = new PdfPage();
        file.AddPage(page);

        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 20, XFontStyleEx.Bold);
        gfx.DrawString("Hello, World!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

        var image = XImage.FromFile("Resources/image.jpg");
        gfx.DrawImage(image, page.Width / 4, 0, image.PixelWidth / 8, image.PixelHeight / 8);

        gfx.Save();
        file.Save("testing.pdf");
    }
}