using Refit;

namespace FileTesting.Client;

public interface IFileTestingApi
{
    [Multipart]
    [Post("/Images")]
    Task<UploadResult> UploadImage([AliasAs("file")] StreamPart stream);

    [Get("/Images/{id}")]
    Task<ImageMetadata> GetImage(Guid id);

    [Get("/Images")]
    Task<List<ImageMetadata>> GetAllImages();

    /// <summary>
    /// Diagnoses an image for corruption and issues.
    /// </summary>
    [Multipart]
    [Post("/Images/diagnose")]
    Task<ImageDiagnosticResult> DiagnoseImage([AliasAs("file")] StreamPart stream);

    /// <summary>
    /// Repairs a corrupt image and returns detailed information.
    /// </summary>
    [Multipart]
    [Post("/Images/repair/details")]
    Task<RepairDetailsResult> RepairImageWithDetails(
        [AliasAs("file")] StreamPart stream,
        [Query] bool stripProfiles = true,
        [Query] bool stripMetadata = false);

    /// <summary>
    /// Repairs a corrupt image and returns the fixed image file.
    /// </summary>
    [Multipart]
    [Post("/Images/repair")]
    Task<HttpResponseMessage> RepairImage(
        [AliasAs("file")] StreamPart stream,
        [Query] bool stripProfiles = true,
        [Query] bool stripMetadata = false);
}