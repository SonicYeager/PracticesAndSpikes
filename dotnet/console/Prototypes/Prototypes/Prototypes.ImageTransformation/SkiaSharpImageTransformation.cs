using System.Diagnostics;
using SkiaSharp;

namespace Prototypes.ImageTransformation;

public sealed class SkiaSharpImageTransformation
{
    public static void TransformImage(string inputPath, string outputPath, Resolution resolution)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.Create(outputPath);
        Stopwatch stopwatch = new();

        stopwatch.Start();
        using var skiaStream = new SKManagedStream(inputStream);
        using var bitmap = SKBitmap.Decode(skiaStream);

        // Calculate the aspect ratio
        var aspectRatio = (float)bitmap.Width / bitmap.Height;
        int newWidth, newHeight;

        if (bitmap.Width > bitmap.Height)
        {
            newWidth = resolution.Width;
            newHeight = (int)(resolution.Width / aspectRatio);
        }
        else
        {
            newHeight = resolution.Height;
            newWidth = (int)(resolution.Height * aspectRatio);
        }

        using var resized = bitmap.Resize(new SKSizeI(newWidth, newHeight),
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
        stopwatch.Stop();

        Console.WriteLine(
            "SKIASHARP:" +
            $" Image transformation took '{stopwatch.ElapsedMilliseconds}' ms for file size '{inputStream.Length / 1024}' kilobytes");

        resized.Encode(outputStream, SKEncodedImageFormat.Png, 5);
    }
}