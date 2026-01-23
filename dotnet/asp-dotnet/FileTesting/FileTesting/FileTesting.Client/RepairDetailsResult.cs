namespace FileTesting.Client;

/// <summary>
/// Detailed result of image repair (without byte array).
/// </summary>
public sealed class RepairDetailsResult
{
    public bool WasRepaired { get; set; }
    public List<string> AppliedFixes { get; set; } = [];
    public ImageDiagnosticResult? OriginalDiagnostics { get; set; }
    public ImageDiagnosticResult? PostRepairDiagnostics { get; set; }
    public int? RepairedSizeBytes { get; set; }
    public string? ErrorMessage { get; set; }
}
