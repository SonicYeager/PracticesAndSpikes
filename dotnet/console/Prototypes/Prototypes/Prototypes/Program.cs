// See https://aka.ms/new-console-template for more information

using Prototypes.ImageTransformation;

var sourceFilePaths = new List<string>
{
    "battleship-gc6fb87054_1280.jpg",
    "frog-1415516.png",
};

const string resourceDirectory = "Resources";
const string skiaSharpOutput = "Resources/Output/SkiaSharp";
const string magickOutput = "Resources/Output/Magick";

if (!Directory.Exists(skiaSharpOutput))
{
    Directory.CreateDirectory(skiaSharpOutput);
}

if (!Directory.Exists(magickOutput))
{
    Directory.CreateDirectory(magickOutput);
}

foreach (var file in sourceFilePaths)
{
    if (File.Exists($"{skiaSharpOutput}/{file}"))
    {
        File.Delete($"{skiaSharpOutput}/{file}");
    }

    if (File.Exists($"{magickOutput}/{file}"))
    {
        File.Delete($"{magickOutput}/{file}");
    }
}

foreach (var file in sourceFilePaths)
{
    SkiaSharpImageTransformation.TransformImage(
        $"{resourceDirectory}/{file}",
        $"{skiaSharpOutput}/{file}",
        new(300, 300));
}

foreach (var file in sourceFilePaths)
{
    MagickImageTransformation.TransformImage(
        $"{resourceDirectory}/{file}",
        $"{magickOutput}/{file}",
        new(300, 300));
}