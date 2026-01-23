namespace FileTesting.Services;

public interface IScalingService
{
    /// <summary>
    /// Scales an image using SkiaSharp with Magick.NET fallback.
    /// </summary>
    (byte[] ImageBytes, int Width, int Height, string Method) ScaleImageAsync(byte[] inputBytes, int maxWidth, int maxHeight);

    /// <summary>
    /// Scales an image using only Magick.NET.
    /// </summary>
    (byte[] ImageBytes, int Width, int Height, string Method) ScaleWithMagickOnly(byte[] inputBytes, int maxWidth, int maxHeight);

    /// <summary>
    /// Gets image dimensions using Magick.NET (more robust than SkiaSharp for corrupt images).
    /// </summary>
    (int Width, int Height) GetDimensionsWithMagick(byte[] inputBytes);
}