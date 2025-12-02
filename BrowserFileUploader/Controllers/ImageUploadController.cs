using BrowserFileUploader.Data;
using BrowserFileUploader.Interfaces;
using BrowserFileUploader.Models;
using BrowserFileUploader.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrowserFileUploader.Controllers
{
    public class ImageUploadController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IImageStorageService _storageService;
        private readonly IConfiguration _configuration;

        public ImageUploadController(AppDbContext dbContext, IImageStorageService storageService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _storageService = storageService;
            _configuration = configuration;
        }

        [HttpGet("")]
        [HttpGet("upload")]
        public IActionResult Upload()
        {
            return View(new ImageUploadViewModel());
        }

        [HttpPost("upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(ImageUploadViewModel model)
        {
            //validate the uploaded image
            model = await Helpers.ImageValidator.ValidateImageUpload(model);

            if(!model.Success) return View(model);

            try
            {
                await using var stream = model.File!.OpenReadStream();

                var (width, height) = await ImageProcessingService.GetDimensionsAsync(stream);

                stream.Position = 0;
                var fileBytes = await ImageProcessingService.LoadAsync(stream);
                var storedContentType = model.File.ContentType ?? "application/octet-stream";

                // store image
                var storedLocation = await _storageService.SaveImageAsync(
                    model.File.FileName,
                    storedContentType,
                    fileBytes);

                var storageMode = _configuration.GetValue<string>("Storage:Mode") ?? Consts.Consts.DEFAULT_STORAGE_MODE;

                //save file reference in the db
                var entity = new ImageUploadReference
                {
                    OriginalFileName = model.File.FileName,
                    StoredFileName = System.IO.Path.GetFileName(storedLocation),
                    ContentType = storedContentType,
                    FileSizeBytes = fileBytes.Length,
                    Location = storedLocation,
                    StorageMode = storageMode
                };

                _dbContext.UploadedImages.Add(entity);
                await _dbContext.SaveChangesAsync();

                model.Success = true;
                model.Message = "Image uploaded successfully.";
                model.ImageUrl = storedLocation.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? storedLocation
                    : Url.Content(storedLocation);

                return View(model);
            }
            catch (Exception ex) {
                model.Success = false;
                model.Message = "An error occurred while uploading the image.";
                return View(model);
            }
        }
    }
}
