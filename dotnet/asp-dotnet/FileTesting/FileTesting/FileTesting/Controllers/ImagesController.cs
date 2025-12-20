using FileTesting.Data;
using FileTesting.Models;
using FileTesting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileTesting.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IScalingService _scalingService;
    private readonly IMinioClient _minioClient;
    private const string BucketName = "images";

    public ImagesController(AppDbContext context, IScalingService scalingService, IMinioClient minioClient)
    {
        _context = context;
        _scalingService = scalingService;
        _minioClient = minioClient;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var inputBytes = memoryStream.ToArray();

        // 1. Scale Image
        var (scaledBytes, width, height, method) = _scalingService.ScaleImageAsync(inputBytes, 300, 300);

        // 2. Prepare Metadata
        using var originalStream = new MemoryStream(inputBytes); // Re-open for upload
        using var scaledStream = new MemoryStream(scaledBytes);

        var id = Guid.NewGuid();
        var originalFileName = $"{id}_original{Path.GetExtension(file.FileName)}";
        var scaledFileName = $"{id}_scaled.jpg"; // We encoded as Jpeg

        // 3. Upload to MinIO
        await _minioClient.UploadFileAsync(BucketName, originalFileName, originalStream, file.ContentType);
        await _minioClient.UploadFileAsync(BucketName, scaledFileName, scaledStream, "image/jpeg");

        // 4. Save to DB
        int origW = 0, origH = 0;
        try
        {
            using var cur = SkiaSharp.SKBitmap.Decode(inputBytes);
            origW = cur.Width;
            origH = cur.Height;
        }
        catch
        {
            // ignored
        }

        var originalUrl = await _minioClient.GetFileUrlAsync(BucketName, originalFileName);
        var scaledUrl = await _minioClient.GetFileUrlAsync(BucketName, scaledFileName);

        var metadata = new ImageMetadata
        {
            Id = id,
            OriginalWidth = origW,
            OriginalHeight = origH,
            ScaledWidth = width,
            ScaledHeight = height,
            Quality = 90, // Hardcoded in ScalingService
            ScalingMethod = method,
            OriginalMinioPath = originalUrl,
            ScaledMinioPath = scaledUrl,
            CreatedAt = DateTime.UtcNow,
        };

        _context.ImageMetadata.Add(metadata);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true, id = id, originalUrl, scaledUrl,
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        var meta = await _context.ImageMetadata.FindAsync(id);
        if (meta == null) return NotFound();

        return Ok(meta);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllImages()
    {
        var images = await _context.ImageMetadata.ToListAsync();
        return Ok(images);
    }
}