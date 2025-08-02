
namespace AdvertisingPlatforms.Base.Constants
{
    public static class ErrorMessages
    {
        public const string FILE_IS_REQUIRED = "File validation failed: File is required.";
        public const string ALLOWED_FILE_EXTENSION = "File validation failed: Only .txt files are allowed.";
        public const string INVALID_FILE_TYPE = "File validation failed: Invalid file type.";
        public const string NULL_LOCATION = "Location cannot be null.";
        public const string EMPTY_LOCATION_PATH = "Location path cannot be empty.";
        public const string UPLOAD_DATA_FAILED = "Upload data failed.";
        public const string NULL_PLATFORMS_COLLECTION = "Collection of platforms cannot by null.";
        public const string EMPTY_PLATFORMS_COLLECTION = "Collection of platforms cannot by empty.";
        public const string ERROR_PROCESSING_PLATFORM = "Error processing platform {PlatformName}.";
        public const string EMPTY_PLATFORM_NAME = "Platform name cannot be empty.";
        public const string EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM = "Platform must have at least one location.";
        public const string ERROR_PARSING_LINE = "Error when parsing a line {NumberLine}: {Line}.";
        public const string ENTITY_CAN_NOT_BE_NULL = "Entity can not be null!";
        public const string ENTITY_NOT_FOUND = "Entity not found!";
        public const string ENTITY_ALREADY_EXISTS = "Entity already exists!";
        public const string ENTITY_COLLECTION_IS_EMPTY = "Collection of entities is empty.";
        public const string NO_DATA_TO_DOWNLOAD = "There is no data to download. The parser returned an empty collection.";
    }
}
