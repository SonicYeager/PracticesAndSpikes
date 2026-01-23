namespace FileTesting.Client;

/// <summary>
/// Result of image diagnostics analysis (mirrors server model).
/// </summary>
public sealed class ImageDiagnosticResult
{
    public bool IsCorrupt { get; set; }
    public bool HasIccIssues { get; set; }
    public bool IsRepairable { get; set; }
    public List<string> Issues { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public string? DetectedFormat { get; set; }
    public string? FileName { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}