using FileTesting.Models;

namespace FileTesting.Services;

/// <summary>
/// Service for diagnosing and repairing corrupt images using Magick.NET.
/// </summary>
public interface IImageRepairService
{
    /// <summary>
    /// Diagnoses an image for corruption and issues.
    /// </summary>
    /// <param name="imageBytes">The image bytes to analyze.</param>
    /// <param name="fileName">Optional file name for format detection.</param>
    /// <returns>Diagnostic result with issues and warnings.</returns>
    ImageDiagnosticResult Diagnose(byte[] imageBytes, string? fileName = null);

    /// <summary>
    /// Attempts to repair a corrupt image.
    /// </summary>
    /// <param name="imageBytes">The image bytes to repair.</param>
    /// <param name="options">Repair options (optional).</param>
    /// <returns>Repair result with fixed image bytes if successful.</returns>
    ImageRepairResult Repair(byte[] imageBytes, RepairOptions? options = null);
}
