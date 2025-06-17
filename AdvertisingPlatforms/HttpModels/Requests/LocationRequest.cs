namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class LocationRequest
    {
        public string Path { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
} 