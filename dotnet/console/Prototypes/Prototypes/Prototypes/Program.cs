// See https://aka.ms/new-console-template for more information

using Prototypes.ImageTransformation;

var sourceFilePaths = new List<string>
{
    "battleship-gc6fb87054_1280.jpg",
    "boats-gb2dac7499_1920.jpg",
    "coffee-gfd73d0f4d_1920.jpg",
    "edward-chou-TXV1rp7ikIo-unsplash.jpg",
    "port-gbc06f731e_1920.jpg",
    "sailing-ship-gba5f7d17c_1920.jpg",
    "wall-gc0452b8bd_1280.jpg",
    "yellow-g32be8ba80_1920.jpg",
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