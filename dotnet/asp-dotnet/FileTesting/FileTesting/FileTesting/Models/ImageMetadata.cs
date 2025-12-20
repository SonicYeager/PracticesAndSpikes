using System.ComponentModel.DataAnnotations;

namespace FileTesting.Models;

public sealed class ImageMetadata
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }

    public int ScaledWidth { get; set; }
    public int ScaledHeight { get; set; }

    public int Quality { get; set; }

    public string ScalingMethod { get; set; } = string.Empty; // "SkiaSharp" or "Magick"

    public string OriginalMinioPath { get; set; } = string.Empty;
    public string ScaledMinioPath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}