namespace BrowserFileUploader.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(string fileName, string contentType, byte[] data, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(string storedLocation, CancellationToken cancellationToken = default);
    }
}
