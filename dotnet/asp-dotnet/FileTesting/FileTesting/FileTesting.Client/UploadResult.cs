namespace FileTesting.Client;

public sealed record UploadResult
{
    public bool Success { get; set; }
    public Guid Id { get; set; }
    public string OriginalUrl { get; set; } = null!;
    public string ScaledUrl { get; set; } = null!;
}