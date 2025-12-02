namespace BrowserFileUploader.Models
{
    public class ImageUploadViewModel
    {
        public IFormFile? File { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }
    }
}
