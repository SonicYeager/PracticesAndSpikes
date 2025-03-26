using System.Diagnostics;
using ImageMagick;

namespace Prototypes.ImageTransformation;

public sealed class MagickImageTransformation
{
    public static void TransformImage(string inputPath, string outputPath, Resolution resolution)
    {
        using var inputStream = File.OpenRead(inputPath);
        using var outputStream = File.Create(outputPath);
        Stopwatch stopwatch = new();

        stopwatch.Start();
        using var magickImage = new MagickImage(inputStream);
        magickImage.Resize((uint)resolution.Width, (uint)resolution.Height);
        magickImage.Quality = 100;
        stopwatch.Stop();

        Console.WriteLine(
            "MAGICK:" +
            $" Image transformation took '{stopwatch.ElapsedMilliseconds}' ms for file size '{inputStream.Length / 1024}' kilobytes");

        magickImage.Write(outputStream);
    }
}