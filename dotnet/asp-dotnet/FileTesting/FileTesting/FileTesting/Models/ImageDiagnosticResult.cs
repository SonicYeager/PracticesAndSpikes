namespace FileTesting.Models;

/// <summary>
/// Result of image diagnostics analysis.
/// </summary>
public sealed class ImageDiagnosticResult
{
    /// <summary>
    /// Indicates if the image has critical corruption that prevents processing.
    /// </summary>
    public bool IsCorrupt { get; set; }

    /// <summary>
    /// Indicates if the image has ICC profile issues (e.g., invalid ICC_ chunks).
    /// </summary>
    public bool HasIccIssues { get; set; }

    /// <summary>
    /// Indicates if the image can be repaired.
    /// </summary>
    public bool IsRepairable { get; set; }

    /// <summary>
    /// List of critical issues found in the image.
    /// </summary>
    public List<string> Issues { get; set; } = [];

    /// <summary>
    /// List of non-critical warnings found in the image.
    /// </summary>
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Detected image format (e.g., PNG, JPEG, GIF).
    /// </summary>
    public string? DetectedFormat { get; set; }

    /// <summary>
    /// Original file name if provided.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Image dimensions if readable.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Image dimensions if readable.
    /// </summary>
    public int? Height { get; set; }
}
