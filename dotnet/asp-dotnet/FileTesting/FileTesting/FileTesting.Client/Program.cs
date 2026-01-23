using System.Diagnostics;
using FileTesting.Client;
using ImageMagick;
using Refit;
using SkiaSharp;

// Config
const string apiBaseUrl = "https://localhost:5001"; // Adjusted to match Kestrel default
// Bypass SSL certificate validation for localhost (common in development)
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = static (message, cert, chain, errors) => true,
};

var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri(apiBaseUrl),
};
var api = RestService.For<IFileTestingApi>(httpClient);

Console.WriteLine($"Starting Load Client against {apiBaseUrl}...");

// 1. Prepare Images
const string testImagesDir = "TestImages";
if (!Directory.Exists(testImagesDir))
{
    Directory.CreateDirectory(testImagesDir);
}

var images = Directory.GetFiles(testImagesDir, "*.jpg")
    .Concat(Directory.GetFiles(testImagesDir, "*.png"))
    .ToArray();

if (images.Length == 0)
{
    Console.WriteLine("No images found. Generating dummy images...");
    Console.WriteLine("Using SkiaSharp for JPEG generation...");
    GenerateDummyImagesWithSkia(testImagesDir, 3);
    Console.WriteLine("Using Magick.NET for PNG generation...");
    GenerateDummyImagesWithMagick(testImagesDir, 3);
    images = Directory.GetFiles(testImagesDir, "*.jpg")
        .Concat(Directory.GetFiles(testImagesDir, "*.png"))
        .ToArray();
}

Console.WriteLine($"Loaded {images.Length} images.");

// 2. Load Loop
long requestCount = 0;
var random = new Random();

while (true)
{
    try
    {
        var imagePath = images[random.Next(images.Length)];
        Console.WriteLine($"\n--- Request {++requestCount} ({Path.GetFileName(imagePath)}) ---");

        // 1. DIAGNOSE
        var sw = Stopwatch.StartNew();
        await using (var diagStream = File.OpenRead(imagePath))
        {
            var diagPart = new StreamPart(diagStream, Path.GetFileName(imagePath), "image/png");
            var diagResult = await api.DiagnoseImage(diagPart);
            sw.Stop();
            Console.WriteLine($"POST /Images/diagnose : {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Format: {diagResult.DetectedFormat} | Size: {diagResult.Width}x{diagResult.Height}");
            Console.WriteLine(
                $"  Corrupt: {diagResult.IsCorrupt} | ICC Issues: {diagResult.HasIccIssues} | Repairable: {diagResult.IsRepairable}");
            if (diagResult.Warnings.Count > 0)
            {
                Console.WriteLine($"  Warnings: {string.Join(", ", diagResult.Warnings)}");
            }
        }

        // 2. UPLOAD
        sw.Restart();
        await using (var stream = File.OpenRead(imagePath))
        {
            var streamPart = new StreamPart(stream, Path.GetFileName(imagePath), "image/jpeg");
            var uploadResult = await api.UploadImage(streamPart);
            sw.Stop();
            Console.WriteLine($"POST /Images : {sw.ElapsedMilliseconds}ms | ID: {uploadResult.Id}");

            // 3. GET DETAILS
            sw.Restart();
            var metadata = await api.GetImage(uploadResult.Id);
            sw.Stop();
            Console.WriteLine(
                $"GET /Images/{uploadResult.Id} : {sw.ElapsedMilliseconds}ms | {metadata.OriginalWidth}x{metadata.OriginalHeight} -> {metadata.ScaledWidth}x{metadata.ScaledHeight} ({metadata.ScalingMethod})");
        }

        // 4. GET LIST
        sw.Restart();
        var all = await api.GetAllImages();
        sw.Stop();
        Console.WriteLine($"GET /Images : {sw.ElapsedMilliseconds}ms | Total Images: {all.Count}");

        // 5. REPAIR TEST (every 5 requests)
        if (requestCount % 5 == 0)
        {
            sw.Restart();
            await using var repairStream = File.OpenRead(imagePath);
            var repairPart = new StreamPart(repairStream, Path.GetFileName(imagePath), "image/png");
            var repairResult = await api.RepairImageWithDetails(repairPart);
            sw.Stop();
            Console.WriteLine($"POST /Images/repair/details : {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Repaired: {repairResult.WasRepaired} | Fixes: {string.Join(", ", repairResult.AppliedFixes)}");
            if (repairResult.RepairedSizeBytes.HasValue)
            {
                Console.WriteLine($"  Repaired size: {repairResult.RepairedSizeBytes} bytes");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        await Task.Delay(1000); // Backoff on error
    }
}

/// <summary>
/// Generates dummy test images using SkiaSharp (JPEG).
/// </summary>
static void GenerateDummyImagesWithSkia(string dir, int count)
{
    var rnd = new Random();
    for (var i = 0; i < count; i++)
    {
        var width = rnd.Next(800, 2000);
        var height = rnd.Next(600, 1500);
        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        // Background
        canvas.Clear(new SKColor((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)));

        // Random shapes
        using var paint = new SKPaint();
        paint.Color = SKColors.White;
        paint.IsAntialias = true;
        for (var j = 0; j < 10; j++)
        {
            paint.Color = new SKColor((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            canvas.DrawCircle(rnd.Next(width), rnd.Next(height), rnd.Next(50, 200), paint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 80);
        using var fileStream = File.OpenWrite(Path.Combine(dir, $"skia_dummy_{i}.jpg"));
        data.SaveTo(fileStream);

        Console.WriteLine($"  Created: skia_dummy_{i}.jpg ({width}x{height})");
    }
}

/// <summary>
/// Generates dummy test images using Magick.NET (PNG).
/// </summary>
static void GenerateDummyImagesWithMagick(string dir, int count)
{
    var rnd = new Random();
    for (var i = 0; i < count; i++)
    {
        var width = rnd.Next(800, 2000);
        var height = rnd.Next(600, 1500);

        // Create image with random color background using hex string
        var bgColor = GetRandomColorHex(rnd);
        using var image = new MagickImage(MagickColors.White, (uint)width, (uint)height);

        // Fill with background color
        image.Evaluate(Channels.Red, EvaluateOperator.Set, rnd.Next(256) * 257);
        image.Evaluate(Channels.Green, EvaluateOperator.Set, rnd.Next(256) * 257);
        image.Evaluate(Channels.Blue, EvaluateOperator.Set, rnd.Next(256) * 257);

        // Add some visual interest with noise and blur
        image.AddNoise(NoiseType.Gaussian);
        image.Blur(0, 3);

        // Save as PNG
        image.Format = MagickFormat.Png;
        var filePath = Path.Combine(dir, $"magick_dummy_{i}.png");
        image.Write(filePath);

        Console.WriteLine($"  Created: magick_dummy_{i}.png ({width}x{height})");
    }
}

static string GetRandomColorHex(Random rnd)
{
    return $"#{rnd.Next(256):X2}{rnd.Next(256):X2}{rnd.Next(256):X2}";
}