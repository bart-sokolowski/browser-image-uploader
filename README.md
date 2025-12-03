Application Structure
Controller

The application exposes a single controller with two endpoints:

GET – Returns the upload view.

POST – Handles image submission, validation, saving, and response display.

ViewModel

Image data is transferred using the ImageUploadViewModel, which contains:

The uploaded file (IFormFile)

Validation and error messages

Upload success state

The resulting image URL or path

This provides a clean separation between the UI and controller logic.

Storage Modes

The app can operate in two modes:

Azure – Images are stored in an Azure Blob Storage container.

FileSystem – Images are stored locally (e.g., wwwroot/uploads).

Two services implement the IImageStorageService interface:

AzureBlobImageStorageService

FileSystemImageStorageService

Both services implement the SaveImageAsync method, allowing the application to switch storage implementations without modifying controller logic. This design makes the storage layer extensible and easily replaceable.

Storage mode is determined using configuration and environment variables.

Image Processing

The project uses the SixLabors.ImageSharp package for safe and cross-platform image handling. It is used to:

Load images without locking the filesystem

Read image metadata such as width and height

Support validation logic that ensures uploaded images do not exceed the maximum allowed dimensions (1024x1024)

Validation Flow

When an image is uploaded, the following validation steps are performed:

Check that a file is provided

Check for allowed content types (JPG or PNG)

Check for allowed file extensions

Check that image dimensions do not exceed 1024x1024

If validation fails, the appropriate error message is placed in the ViewModel and displayed on the upload view.

If validation passes:

The image is loaded and saved using the selected storage mode

A database record is created for the upload

Database Storage

Uploaded images are logged in a local SQLite database using Entity Framework Core.

The AppDbContext is configured to use SQLite

The ImageUploadReference model stores metadata such as:

Original file name

Stored file name

Content type

Image dimensions

Storage mode used

Upload timestamp

A migration was created and applied to generate the SQLite database schema locally.

User Interface

After a successful upload:

The stored image reference (URL or local path) is passed back through the ViewModel

The image is rendered in the Razor view (Upload.cshtml) along with a success message

Configuration

Settings are managed through appsettings.json and environment-specific overrides. The relevant configuration structure is:

"ConnectionStrings": {
  "DefaultConnection": "Data Source=app.db;",
  "AzureBlobStorage": "AZURE_STORAGE_KEY"
},

"Storage": {
  "Mode": "FILE_SYSTEM or AZURE",
  "LocalPath": "uploads"
},

"Azure": {
  "ContainerName": "DEFAULT_CONTAINER_NAME"
}
