namespace FileTesting.Models;

/// <summary>
/// Result of an image repair operation.
/// </summary>
public sealed class ImageRepairResult
{
    /// <summary>
    /// Indicates if any repairs were applied to the image.
    /// </summary>
    public bool WasRepaired { get; set; }

    /// <summary>
    /// The repaired image bytes, or null if repair failed.
    /// </summary>
    public byte[]? RepairedBytes { get; set; }

    /// <summary>
    /// List of fixes that were applied to the image.
    /// </summary>
    public List<string> AppliedFixes { get; set; } = [];

    /// <summary>
    /// Diagnostic results from before the repair.
    /// </summary>
    public ImageDiagnosticResult OriginalDiagnostics { get; set; } = new();

    /// <summary>
    /// Diagnostic results after the repair (if successful).
    /// </summary>
    public ImageDiagnosticResult? PostRepairDiagnostics { get; set; }

    /// <summary>
    /// Error message if repair failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
