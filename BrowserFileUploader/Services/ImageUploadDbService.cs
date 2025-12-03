using BrowserFileUploader.Data;
using BrowserFileUploader.Interfaces;
using BrowserFileUploader.Models;

namespace BrowserFileUploader.Services
{
    public class ImageUploadDbService : IImageUploadDbService
    {

        private readonly AppDbContext _db;

        public ImageUploadDbService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<bool> SaveUploadRecordAsync(ImageUploadReference entity)
        {
            try
            {
                _db.UploadedImages.Add(entity);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                //log the db error to the monitoring service
                return false;
            }
        }
    }
}
