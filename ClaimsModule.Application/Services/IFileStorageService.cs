using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Services;

/// <summary>
/// Interface used for uploading files to a persistent storage service (e.g., MinIO).
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file stream to the storage system with the specified object name and content type.
    /// </summary>
    /// <param name="stream">The file stream to be uploaded.</param>
    /// <param name="objectName">The unique name or path to be used when storing the object.</param>
    /// <param name="contentType">The MIME type of the file (e.g., image/png, application/pdf).</param>
    /// <returns>A task that returns the public or presigned URL of the uploaded object.</returns>
    Task<string> UploadAsync(Stream stream, string objectName, string contentType);
}