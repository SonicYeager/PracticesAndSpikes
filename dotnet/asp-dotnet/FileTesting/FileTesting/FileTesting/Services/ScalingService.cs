using ImageMagick;
using SkiaSharp;

namespace FileTesting.Services;

public sealed class ScalingService : IScalingService
{
    public (byte[] ImageBytes, int Width, int Height, string Method) ScaleImageAsync(byte[] inputBytes, int maxWidth,
        int maxHeight)
    {
        try
        {
            return ScaleWithSkia(inputBytes, maxWidth, maxHeight);
        }
        catch (Exception)
        {
            // Fallback to Magick
            return ScaleWithMagick(inputBytes, maxWidth, maxHeight);
        }
    }

    private (byte[] ImageBytes, int Width, int Height, string Method) ScaleWithSkia(byte[] inputBytes, int maxWidth, int maxHeight)
    {
        using var inputStream = new MemoryStream(inputBytes);
        using var original = SKBitmap.Decode(inputStream);

        if (original == null) throw new InvalidOperationException("Skia failed to decode");

        var ratioX = (double)maxWidth / original.Width;
        var ratioY = (double)maxHeight / original.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(original.Width * ratio);
        var newHeight = (int)(original.Height * ratio);

        // SkiaSharp 3.x uses SKSamplingOptions
        using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), new SKSamplingOptions(SKCubicResampler.Mitchell));
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        return (data.ToArray(), newWidth, newHeight, "SkiaSharp");
    }

    private static (byte[] ImageBytes, int Width, int Height, string Method) ScaleWithMagick(byte[] inputBytes, int maxWidth, int maxHeight)
    {
        using var image = new MagickImage(inputBytes);
        image.Resize((uint)maxWidth, (uint)maxHeight); // Magick maintains aspect ratio by default with this overload
        image.Quality = 90; // Properties usually handle int->uint implicitly or are int?
        // If Quality is uint, 90 literal works.
        // Let's verify Resize first.
        image.Format = MagickFormat.Jpeg;

        return (image.ToByteArray(), (int)image.Width, (int)image.Height, "Magick.NET");
    }
}