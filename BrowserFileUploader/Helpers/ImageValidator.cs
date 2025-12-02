using BrowserFileUploader.Models;
using BrowserFileUploader.Services;

namespace BrowserFileUploader.Helpers
{
    public class ImageValidator
    {
        public static async Task<ImageUploadViewModel> ValidateImageUpload(ImageUploadViewModel model)
        {

            //base image check
            if (model.File == null || model.File.Length == 0)
            {
                model.Success = false;
                model.Message = "Please select an image to upload.";

                return model;
            }

            //check content type
            if (!ImageProcessingService.IsAllowedContentType(model.File.ContentType) ||
                !ImageProcessingService.HasAllowedExtension(model.File.FileName))
            {
                model.Success = false;
                model.Message = "Only JPG and PNG images are allowed.";

                return model;
            }

            //check image dimensions
            await using (var stream = model.File.OpenReadStream())
            {
                var (width, height) = await ImageProcessingService.GetDimensionsAsync(stream);
                var maxDimensions = Consts.Consts.MAX_DIMENSIONS;

                if (width > maxDimensions ||
                    height > maxDimensions)
                {
                    model.Success = false;
                    model.Message = $"Image dimensions exceed the maximum allowed size of {maxDimensions}x{maxDimensions}.";
                    return model;
                }
            }

            model.Success = true;

            return model;
        }
    }
}
