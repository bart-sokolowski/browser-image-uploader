
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace BrowserFileUploader.Services
{
    public class ImageProcessingService
    {
        public static bool IsAllowedContentType(string? contentType)
        {
            if (contentType == null)
            {
                return false;
            }

            foreach (var allowed in Consts.Consts.ALLOWED_CONTENT_TYPES)
            {
                if (string.Equals(contentType, allowed, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }  
            }

            return false;
        }

        public static bool HasAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png";
        }

        public static async Task<(int Width, int Height)> GetDimensionsAsync(Stream input)
        {
            input.Position = 0;
            using var image = await Image.LoadAsync(input);
            return (image.Width, image.Height);
        }

        public static async Task<byte[]> LoadAsync(Stream input)
        {
            input.Position = 0;

            using var stream = new MemoryStream();
            await input.CopyToAsync(stream);
            return stream.ToArray();
        }
    }
}
