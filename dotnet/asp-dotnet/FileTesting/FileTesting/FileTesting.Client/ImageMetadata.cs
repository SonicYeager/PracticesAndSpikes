namespace FileTesting.Client;

public sealed record ImageMetadata
{
    public Guid Id { get; set; }
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }
    public int ScaledWidth { get; set; }
    public int ScaledHeight { get; set; }
    public string OriginalMinioPath { get; set; } = null!;
    public string ScaledMinioPath { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}