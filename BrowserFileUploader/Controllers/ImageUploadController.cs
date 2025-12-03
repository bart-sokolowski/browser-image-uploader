using BrowserFileUploader.Data;
using BrowserFileUploader.Interfaces;
using BrowserFileUploader.Models;
using BrowserFileUploader.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrowserFileUploader.Controllers
{
    public class ImageUploadController : Controller
    {
        private readonly IImageUploadDbService _dbService;
        private readonly IImageStorageService _storageService;
        private readonly IConfiguration _configuration;

        public ImageUploadController(IImageUploadDbService dbService, IImageStorageService storageService, IConfiguration configuration)
        {
            _dbService = dbService;
            _storageService = storageService;
            _configuration = configuration;
        }

        [HttpGet("")]
        [HttpGet("upload")]
        public IActionResult Index()
        {
            return View(new ImageUploadViewModel());
        }

        [HttpPost("upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ImageUploadViewModel model)
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

                

                //check if the db succeed to store the file reference to complete the transaction
                //if not, remove the stored image
                if (!await _dbService.SaveUploadRecordAsync(entity))
                {
                    try
                    {
                        await _storageService.DeleteImageAsync(storedLocation);
                    }
                    catch
                    {
                        // log the error to the monitoring service like Azure Logger
                    }

                    model.Success = false;
                    model.Message = "The image upload was successfull, but we could not record it in the database. Please try again later.";

                    return View(model);
                }

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
