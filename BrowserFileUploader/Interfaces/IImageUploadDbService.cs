using BrowserFileUploader.Models;

namespace BrowserFileUploader.Interfaces
{
    public interface IImageUploadDbService
    {
        Task<bool> SaveUploadRecordAsync(ImageUploadReference entity);
    }
}
