using FileTesting.Data;
using FileTesting.Models;
using FileTesting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace FileTesting.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IScalingService _scalingService;
    private readonly IMinioClient _minioClient;
    private readonly IImageRepairService _imageRepairService;
    private const string BucketName = "images";

    public ImagesController(
        AppDbContext context,
        IScalingService scalingService,
        IMinioClient minioClient,
        IImageRepairService imageRepairService)
    {
        _context = context;
        _scalingService = scalingService;
        _minioClient = minioClient;
        _imageRepairService = imageRepairService;
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

        // 4. Get dimensions - use Magick.NET for more robust handling
        int origW = 0, origH = 0;
        try
        {
            (origW, origH) = _scalingService.GetDimensionsWithMagick(inputBytes);
        }
        catch
        {
            // Fallback to SkiaSharp
            try
            {
                using var cur = SKBitmap.Decode(inputBytes);
                origW = cur.Width;
                origH = cur.Height;
            }
            catch
            {
                // ignored
            }
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

    /// <summary>
    /// Diagnoses an uploaded image for corruption and issues.
    /// </summary>
    [HttpPost("diagnose")]
    public async Task<IActionResult> DiagnoseImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var inputBytes = memoryStream.ToArray();

        var result = _imageRepairService.Diagnose(inputBytes, file.FileName);
        return Ok(result);
    }

    /// <summary>
    /// Repairs a corrupt image and returns the fixed version.
    /// </summary>
    [HttpPost("repair")]
    public async Task<IActionResult> RepairImage(IFormFile? file, [FromQuery] bool stripProfiles = true,
        [FromQuery] bool stripMetadata = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var inputBytes = memoryStream.ToArray();

        var options = new RepairOptions
        {
            StripColorProfiles = stripProfiles, StripMetadata = stripMetadata,
        };

        var result = _imageRepairService.Repair(inputBytes, options);

        if (!result.WasRepaired || result.RepairedBytes == null)
        {
            return BadRequest(new
            {
                success = false, error = result.ErrorMessage ?? "Could not repair image", diagnostics = result.OriginalDiagnostics,
            });
        }

        // Return the repaired image as a file download
        const string contentType = "image/png";
        var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_repaired.png";

        return File(result.RepairedBytes, contentType, fileName);
    }

    /// <summary>
    /// Repairs an image and returns detailed information about the repair.
    /// </summary>
    [HttpPost("repair/details")]
    public async Task<IActionResult> RepairImageWithDetails(IFormFile? file, [FromQuery] bool stripProfiles = true,
        [FromQuery] bool stripMetadata = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var inputBytes = memoryStream.ToArray();

        var options = new RepairOptions
        {
            StripColorProfiles = stripProfiles, StripMetadata = stripMetadata,
        };

        var result = _imageRepairService.Repair(inputBytes, options);

        // Don't return the bytes in JSON, just the metadata
        return Ok(new
        {
            wasRepaired = result.WasRepaired,
            appliedFixes = result.AppliedFixes,
            originalDiagnostics = result.OriginalDiagnostics,
            postRepairDiagnostics = result.PostRepairDiagnostics,
            repairedSizeBytes = result.RepairedBytes?.Length,
            errorMessage = result.ErrorMessage,
        });
    }
}