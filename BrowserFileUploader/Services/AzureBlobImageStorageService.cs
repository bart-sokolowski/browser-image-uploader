using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BrowserFileUploader.Interfaces;
using System.Threading;
using System.Xml;

namespace BrowserFileUploader.Services
{
    public class AzureBlobImageStorageService : IImageStorageService
    {
        private readonly BlobContainerClient _blobContainer;

        public AzureBlobImageStorageService(string connectionString, string containerName)
        {
            var serviceClient = new BlobServiceClient(connectionString);

            _blobContainer = serviceClient.GetBlobContainerClient(containerName);
            _blobContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> SaveImageAsync(string fileName, string contentType, byte[] data, CancellationToken cancellationToken = default)
        {
            var blobId = Guid.NewGuid().ToString("N");
            var blobName = $"{blobId}_{fileName}"; 
            var blobClient = _blobContainer.GetBlobClient(blobName);

            await using var stream = new MemoryStream(data);
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken);

            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string storedLocation, CancellationToken cancellationToken = default)
        {
            var blobName = Path.GetFileName(new Uri(storedLocation).AbsolutePath);
            var blobClient = _blobContainer.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
    }
}
