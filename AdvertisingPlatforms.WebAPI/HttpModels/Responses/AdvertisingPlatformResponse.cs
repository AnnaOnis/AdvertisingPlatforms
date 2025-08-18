namespace AdvertisingPlatforms.Web.HttpModels.Responses
{
    public class AdvertisingPlatformResponse
    {
        public Guid Id { get; set; }
        public Guid AdvertisementId { get; set; }
        public Guid LocationId { get; set; }
        public string AdvertisementName { get; set; } = string.Empty;
        public string LocationPath { get; set; } = string.Empty;
    }
}
