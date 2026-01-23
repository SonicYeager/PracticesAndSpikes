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
            return ScaleWithMagickOnly(inputBytes, maxWidth, maxHeight);
        }
    }

    /// <inheritdoc />
    public (byte[] ImageBytes, int Width, int Height, string Method) ScaleWithMagickOnly(byte[] inputBytes, int maxWidth, int maxHeight)
    {
        using var image = new MagickImage(inputBytes);
        image.Resize((uint)maxWidth, (uint)maxHeight); // Magick maintains aspect ratio by default with this overload
        image.Quality = 90;
        image.Format = MagickFormat.Jpeg;

        return (image.ToByteArray(), (int)image.Width, (int)image.Height, "Magick.NET");
    }

    /// <inheritdoc />
    public (int Width, int Height) GetDimensionsWithMagick(byte[] inputBytes)
    {
        using var image = new MagickImage(inputBytes);
        return ((int)image.Width, (int)image.Height);
    }

    private static (byte[] ImageBytes, int Width, int Height, string Method) ScaleWithSkia(byte[] inputBytes, int maxWidth, int maxHeight)
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
}