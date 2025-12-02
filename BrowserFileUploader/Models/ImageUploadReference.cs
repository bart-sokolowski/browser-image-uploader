namespace BrowserFileUploader.Models
{
    public class ImageUploadReference
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string Location { get; set; }
        public string StorageMode { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
