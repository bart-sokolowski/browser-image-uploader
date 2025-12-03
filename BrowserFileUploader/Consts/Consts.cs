namespace BrowserFileUploader.Consts
{
    public class Consts
    {
        public const int MAX_DIMENSIONS = 1024;
        public const string DEFAULT_STORAGE_MODE = "FILE_SYSTEM";
        public const string DEFAULT_FILE_STORAGE_UPLOAD_PATH = "uploads";

        public static readonly string[] ALLOWED_CONTENT_TYPES =
        {
            "image/jpeg",
            "image/jpg",
            "image/png"
        };
    }
}
