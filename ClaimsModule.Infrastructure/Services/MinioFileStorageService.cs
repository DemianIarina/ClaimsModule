using ClaimsModule.Application.Services;
using Minio;
using Minio.DataModel.Args;
using System.IO;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Services;

/// <summary>
/// Provides file storage capabilities using MinIO as the storage.
/// Handles upload and access to files (e.g., images, documents) via presigned URLs.
/// </summary>
public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName = "claims-photos";

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioFileStorageService"/> class.
    /// </summary>
    /// <param name="minioClient">The MinIO client used for performing storage operations.</param>
    public MinioFileStorageService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    /// <inheritdoc/>
    public async Task<string> UploadAsync(Stream stream, string objectName, string contentType)
    {
        var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType));

        return await GeneratePresignedUrlAsync(objectName);
    }

    /// <inheritdoc/>
    public async Task<string> GeneratePresignedUrlAsync(string objectName)
    {
        var presignedUrl = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithExpiry(60 * 60 * 24)); // 1 days

        return presignedUrl;
    }

}
