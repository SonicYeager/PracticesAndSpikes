using FileTesting.Models;
using ImageMagick;

namespace FileTesting.Services;

/// <summary>
/// Magick.NET-based implementation of image repair service.
/// Handles detection and fixing of corrupt image patterns like invalid ICC chunks.
/// </summary>
public sealed class ImageRepairService : IImageRepairService
{
    /// <inheritdoc />
    public ImageDiagnosticResult Diagnose(byte[] imageBytes, string? fileName = null)
    {
        var result = new ImageDiagnosticResult
        {
            FileName = fileName,
        };

        try
        {
            // Try to read the image with Magick.NET
            using var image = new MagickImage();

            // Configure to collect warnings instead of throwing
            image.Warning += (sender, args) =>
            {
                var warning = args.Message;

                // Check for ICC-related warnings
                if (warning.Contains("iCCP", StringComparison.OrdinalIgnoreCase) ||
                    warning.Contains("ICC", StringComparison.OrdinalIgnoreCase) ||
                    warning.Contains("color profile", StringComparison.OrdinalIgnoreCase))
                {
                    result.HasIccIssues = true;
                    result.Warnings.Add($"ICC Issue: {warning}");
                }
                else
                {
                    result.Warnings.Add(warning);
                }
            };

            image.Read(imageBytes);

            // Successfully read - extract info
            result.DetectedFormat = image.Format.ToString();
            result.Width = (int)image.Width;
            result.Height = (int)image.Height;
            result.IsRepairable = true;

            // Check for specific issues
            CheckForKnownIssues(image, result);
        }
        catch (MagickException ex)
        {
            result.IsCorrupt = true;
            result.Issues.Add($"Failed to read image: {ex.Message}");

            // Check if it's a known repairable error
            if (IsRepairableError(ex))
            {
                result.IsRepairable = true;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public ImageRepairResult Repair(byte[] imageBytes, RepairOptions? options = null)
    {
        options ??= new RepairOptions();

        var result = new ImageRepairResult
        {
            OriginalDiagnostics = Diagnose(imageBytes),
        };

        try
        {
            using var image = new MagickImage();

            // Suppress warnings during repair
            image.Warning += static (_, _) => { };

            image.Read(imageBytes);

            // Apply repairs based on options
            if (options.StripColorProfiles)
            {
                var profileCount = RemoveColorProfiles(image);
                if (profileCount > 0)
                {
                    result.AppliedFixes.Add($"Removed {profileCount} color profile(s)");
                }
            }

            if (options.StripMetadata)
            {
                image.Strip();
                result.AppliedFixes.Add("Stripped all metadata");
            }

            // Set quality for lossy formats
            image.Quality = (uint)options.Quality;

            // Re-encode if requested or if there were ICC issues
            if (options.ReEncodeImage || result.OriginalDiagnostics.HasIccIssues)
            {
                var targetFormat = options.TargetFormat ?? image.Format;
                image.Format = targetFormat;
                result.AppliedFixes.Add($"Re-encoded as {targetFormat}");
            }

            result.RepairedBytes = image.ToByteArray();
            result.WasRepaired = result.AppliedFixes.Count > 0;

            // Run post-repair diagnostics
            if (result.RepairedBytes != null)
            {
                result.PostRepairDiagnostics = Diagnose(result.RepairedBytes);
            }
        }
        catch (MagickException ex)
        {
            result.ErrorMessage = $"Repair failed: {ex.Message}";

            // Try aggressive repair for severely corrupt images
            try
            {
                result.RepairedBytes = AggressiveRepair(imageBytes, options);
                if (result.RepairedBytes != null)
                {
                    result.AppliedFixes.Add("Applied aggressive repair (re-encoded from raw pixel data)");
                    result.WasRepaired = true;
                    result.ErrorMessage = null;
                    result.PostRepairDiagnostics = Diagnose(result.RepairedBytes);
                }
            }
            catch
            {
                // Aggressive repair also failed
            }
        }

        return result;
    }

    /// <summary>
    /// Checks for known image issues that can be repaired.
    /// </summary>
    private static void CheckForKnownIssues(MagickImage image, ImageDiagnosticResult result)
    {
        // Check for color profiles
        var profiles = image.GetColorProfile();
        if (profiles != null)
        {
            // Verify the profile is valid
            try
            {
                var data = profiles.ToByteArray();
                if (data == null || data.Length == 0)
                {
                    result.HasIccIssues = true;
                    result.Warnings.Add("Empty or invalid ICC profile detected");
                }
            }
            catch
            {
                result.HasIccIssues = true;
                result.Warnings.Add("Corrupt ICC profile detected");
            }
        }

        // Check for PNG-specific issues
        if (image.Format == MagickFormat.Png)
        {
            // Check for invalid chunk names (like ICC_ instead of iCCP)
            // This is typically caught during read and reported as a warning
        }
    }

    /// <summary>
    /// Determines if an error is potentially repairable.
    /// </summary>
    private static bool IsRepairableError(MagickException ex)
    {
        var message = ex.Message.ToLowerInvariant();

        // Known repairable errors
        return message.Contains("icc") ||
               message.Contains("profile") ||
               message.Contains("crc") ||
               message.Contains("chunk") ||
               message.Contains("corrupt");
    }

    /// <summary>
    /// Removes all color profiles from the image.
    /// </summary>
    private static int RemoveColorProfiles(MagickImage image)
    {
        var count = 0;

        var colorProfile = image.GetColorProfile();
        if (colorProfile != null)
        {
            image.RemoveProfile(colorProfile.Name);
            count++;
        }

        // Also try removing by known profile names
        string[] profileNames = ["icc", "icm", "ICC", "ICM"];
        foreach (var name in profileNames)
        {
            try
            {
                image.RemoveProfile(name);
                count++;
            }
            catch
            {
                // Profile doesn't exist, ignore
            }
        }

        return count;
    }

    /// <summary>
    /// Attempts aggressive repair by re-reading and re-encoding pixel data.
    /// </summary>
    private static byte[]? AggressiveRepair(byte[] imageBytes, RepairOptions options)
    {
        // Try reading with different settings
        var settings = new MagickReadSettings
        {
            ColorSpace = ColorSpace.sRGB,
        };

        using var image = new MagickImage();
        image.Warning += static (_, _) => { }; // Suppress warnings

        // First try: ignore profiles entirely
        image.Read(imageBytes, settings);

        // Remove all profiles
        image.Strip();

        // Set quality and format
        image.Quality = (uint)options.Quality;
        var targetFormat = options.TargetFormat ?? MagickFormat.Png;
        image.Format = targetFormat;

        return image.ToByteArray();
    }
}
