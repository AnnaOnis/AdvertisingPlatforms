namespace AdvertisingPlatforms.Base.Constants
{
    public static class LogMessages
    {
        public const string STARTING_FILE_UPLOAD = "Starting file upload: {FileName}";
        public const string PARSING_FILE_CONTENT = "Parsing file content";
        public const string UPLOADING_PLATFORMS = "Uploading {Count} platforms";
        public const string DATA_UPLOADED_SUCCESSFULLY = "Data uploaded successfully.";
        public const string SEARCH_REQUEST_FOR_LOCATION = "Search request for location: {Location}";
        public const string RETURNING_PLATFORMS_FOR_LOCATION = "Returning {Count} platforms for location: {Location}";
        public const string COUNT_PROCESSED_LOCATIONS = "Processed {PlatformCount} platforms with {LocationPrefixCount} locations";
        
        // Upload data logging
        public const string LOCATION_ALREADY_EXISTS = "Location already exists: {LocationPath}";
        public const string LOCATION_CREATED = "New location created: {LocationPath}";
        public const string LOCATIONS_UPLOAD_SUMMARY = "Locations upload completed. Added: {AddedCount}, Skipped duplicates: {SkippedCount}";
        
        public const string ADVERTISEMENT_ALREADY_EXISTS = "Advertisement already exists: {AdvertisementName}";
        public const string ADVERTISEMENT_CREATED = "New advertisement created: {AdvertisementName}";
        public const string ADVERTISEMENTS_UPLOAD_SUMMARY = "Advertisements upload completed. Added: {AddedCount}, Skipped duplicates: {SkippedCount}";
        
        public const string PLATFORM_ALREADY_EXISTS = "Platform already exists: {AdvertisementName} - {LocationPath}";
        public const string PLATFORM_CREATED = "New platform created: {AdvertisementName} - {LocationPath}";
        public const string PLATFORMS_UPLOAD_SUMMARY = "Platforms upload completed. Added: {AddedCount}, Skipped duplicates: {SkippedCount}";
    }
}
