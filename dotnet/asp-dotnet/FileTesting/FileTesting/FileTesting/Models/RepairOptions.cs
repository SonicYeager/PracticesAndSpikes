using ImageMagick;

namespace FileTesting.Models;

/// <summary>
/// Options for image repair operations.
/// </summary>
public sealed class RepairOptions
{
    /// <summary>
    /// If true, removes all color profiles from the image.
    /// Useful for fixing ICC_ chunk issues.
    /// </summary>
    public bool StripColorProfiles { get; set; } = true;

    /// <summary>
    /// If true, re-encodes the image to fix structural issues.
    /// </summary>
    public bool ReEncodeImage { get; set; } = true;

    /// <summary>
    /// If true, strips all metadata from the image.
    /// </summary>
    public bool StripMetadata { get; set; } = false;

    /// <summary>
    /// Target format for re-encoding. If null, keeps original format.
    /// </summary>
    public MagickFormat? TargetFormat { get; set; }

    /// <summary>
    /// Quality setting for JPEG/WebP encoding (1-100).
    /// </summary>
    public int Quality { get; set; } = 90;
}
