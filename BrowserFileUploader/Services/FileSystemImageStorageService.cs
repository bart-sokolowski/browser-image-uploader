using BrowserFileUploader.Interfaces;

namespace BrowserFileUploader.Services
{
    public class FileSystemImageStorageService : IImageStorageService
    {
        private readonly string _rootPath;
        public FileSystemImageStorageService(string rootPath)
        {
            _rootPath = rootPath;
            Directory.CreateDirectory(_rootPath);
        }
        public async Task<string> SaveImageAsync(string fileName, string contentType, byte[] data, CancellationToken cancellationToken = default)
        {
            string uniqueId = Guid.NewGuid().ToString("N");
            string uniqueName = $"{uniqueId}_{fileName}";
            var fullPath = Path.Combine(_rootPath, uniqueName);

            await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fs.WriteAsync(data, 0, data.Length, cancellationToken);
            }

            var relativePath = $"~/{Consts.Consts.DEFAULT_FILE_STORAGE_UPLOAD_PATH}/{uniqueName}";
            return relativePath;
        }
    }
}
