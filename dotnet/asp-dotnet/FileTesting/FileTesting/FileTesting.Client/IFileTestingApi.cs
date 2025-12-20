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
}