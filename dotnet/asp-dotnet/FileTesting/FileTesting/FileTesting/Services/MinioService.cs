using Amazon.S3;
using Amazon.S3.Model;

namespace FileTesting.Services;

public sealed class MinioService : IMinioClient
{
    private readonly IAmazonS3 _s3Client;

    public MinioService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task UploadFileAsync(string bucketName, string objectName, Stream inputStream, string contentType)
    {
        try
        {
            await _s3Client.PutBucketAsync(bucketName);
        }
        catch (AmazonS3Exception)
        {
            // Ignore if exists or handle check explicitly
        }

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectName,
            InputStream = inputStream,
            ContentType = contentType,
            AutoCloseStream = false,
        };

        await _s3Client.PutObjectAsync(putRequest);
    }

    public Task<string> GetFileUrlAsync(string bucketName, string objectName)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName, Key = objectName, Expires = DateTime.UtcNow.AddHours(24),
        };

        string url = _s3Client.GetPreSignedURL(request);
        // Force HTTP for local MinIO if it generated HTTPS, as MinIO is likely not configured with SSL
        return Task.FromResult(url.Replace("https://", "http://"));
    }
}