namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class UpdateAdvertisementRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
} 