using System.Diagnostics;
using Refit;
using SkiaSharp;
using FileTesting.Client;

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

var images = Directory.GetFiles(testImagesDir, "*.jpg");
if (images.Length == 0)
{
    Console.WriteLine("No images found. Generating dummy images...");
    GenerateDummyImages(testImagesDir, 5);
    images = Directory.GetFiles(testImagesDir, "*.jpg");
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
        Console.WriteLine($"--- Request {++requestCount} ---");

        // 1. UPLOAD
        var sw = Stopwatch.StartNew();
        await using var stream = File.OpenRead(imagePath);
        var streamPart = new StreamPart(stream, Path.GetFileName(imagePath), "image/jpeg");

        var uploadResult = await api.UploadImage(streamPart);
        sw.Stop();
        Console.WriteLine($"POST /Images : {sw.ElapsedMilliseconds}ms | ID: {uploadResult.Id} | Scaled: {uploadResult.ScaledUrl}");

        // 2. GET DETAILS
        sw.Restart();
        var metadata = await api.GetImage(uploadResult.Id);
        sw.Stop();
        Console.WriteLine(
            $"GET /Images/{uploadResult.Id} : {sw.ElapsedMilliseconds}ms | {metadata.OriginalWidth}x{metadata.OriginalHeight} -> {metadata.ScaledWidth}x{metadata.ScaledHeight}");

        // 3. GET LIST
        sw.Restart();
        var all = await api.GetAllImages();
        sw.Stop();
        Console.WriteLine($"GET /Images : {sw.ElapsedMilliseconds}ms | Total Images: {all.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        await Task.Delay(1000); // Backoff on error
    }
}

static void GenerateDummyImages(string dir, int count)
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
        using var fileStream = File.OpenWrite(Path.Combine(dir, $"dummy_{i}.jpg"));
        data.SaveTo(fileStream);
    }
}