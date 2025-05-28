
namespace AdvertisingPlatforms.Base.Constants
{
    public class ErrorMessages
    {
        public const string FILE_VALIDATION_FAILED = "File validation failed: {Error}.";
        public const string FILE_IS_REQUIRED = "File is required.";
        public const string ALLOWED_FILE_EXTENSION = "Only .txt files are allowed.";
        public const string INVALID_FILE_TYPE = "Invalid file type.";
        public const string NULL_LOCATION = "Location cannot be null.";
        public const string EMPTY_LOCATION_PATH = "Location path cannot be empty.";
        public const string UPLOAD_DATA_FAILED = "Upload data failed.";
        public const string NULL_PLATFORMS_COLLECTION = "Collection of platforms cannot by null.";
        public const string EMPTY_PLATFORMS_COLLECTION = "Collection of platforms cannot by empty.";
        public const string ERROR_PROCESSING_PLATFORM = "Error processing platform {PlatformName}.";
        public const string EMPTY_PLATFORM_NAME = "Platform name cannot be empty.";
        public const string EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM = "Platform must have at least one location.";
        public const string ERROR_PARSING_LINE = "Error when parsing a line {NumberLine}: {Line}.";
    }
}
