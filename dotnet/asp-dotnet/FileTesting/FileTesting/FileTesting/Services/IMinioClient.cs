namespace FileTesting.Services;

public interface IMinioClient
{
    Task UploadFileAsync(string bucketName, string objectName, Stream inputStream, string contentType);
    Task<string> GetFileUrlAsync(string bucketName, string objectName); // Returns a pre-signed URL or direct path
}
