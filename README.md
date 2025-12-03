# **Application Structure**

## **Controller**

The application exposes a single controller with two endpoints:

-   **GET** -- Returns the upload view.
-   **POST** -- Handles image submission, validation, saving, and
    response display.

## **ViewModel**

Image data is transferred using the **ImageUploadViewModel**, which
contains:

-   The uploaded file (`IFormFile`)
-   Validation and error messages
-   Upload success state
-   The resulting image URL or path

This provides a clean separation between the UI and controller logic.

# **Storage Modes**

The application supports two image storage implementations:

-   **Azure** -- Images are stored in a provided Azure Blob Storage container.
-   **FileSystem** -- Images are stored locally (
    `wwwroot/uploads`).

Two services implement the **IImageStorageService** interface:

-   `AzureBlobImageStorageService`
-   `FileSystemImageStorageService`

Both services implement `SaveImageAsync`, which allows the application
to switch storage mechanisms without modifying controller logic.\
This design makes the storage layer extensible and easy to replace or
expand.

Storage mode is controlled through configuration and environment
variables.

# **Image Processing**

The project uses the **SixLabors.ImageSharp** library for safe and
cross-platform image handling.\
It is used to:

-   Load images without locking the filesystem\
-   Read metadata like image width and height\
-   Support validation that prevents uploading images exceeding the
    maximum dimensions (1024×1024)

# **Validation Flow**

When an image is uploaded, the following validation checks are
performed:

-   The file exists\
-   The content type is allowed (JPG or PNG)\
-   The file extension is valid\
-   The image dimensions do not exceed **1024 × 1024**

If validation fails, an appropriate error message is added to the
ViewModel and shown on the upload view.

If validation succeeds:

-   The image is loaded and saved using the selected storage mode\
-   A database record for the image upload is created

# **Database Storage**

Uploaded image references are stored in a local SQLite database using
Entity Framework Core.

-   The `AppDbContext` is configured to use SQLite.
-   The `ImageUploadReference` model stores:
    -   Original file name\
    -   Stored file name\
    -   Content type\
    -   Storage mode used\
    -   Upload timestamp

A migration was created and applied to generate the SQLite database
schema.

# **User Interface**

After a successful upload:

-   The stored image URL or local path is added to the ViewModel\
-   The image is displayed in the Razor view (`ImageUpload/Index.cshtml`)\
-   A success message is shown to the user

# **Configuration**

Application settings are managed in **appsettings.json** and
environment-specific overrides.

Example configuration:

    "ConnectionStrings": {
      "DefaultConnection": "Data Source=app.db;",
      "AzureBlobStorage": "AZURE_STORAGE_CONN_STRING"
    },

    "Storage": {
      "Mode": "FILE_SYSTEM or AZURE",
      "LocalPath": "uploads"
    },

    "Azure": {
      "ContainerName": "DEFAULT_CONTAINER_NAME"
    }
