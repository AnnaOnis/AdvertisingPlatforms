namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class UpdateAdvertisingPlatformRequest
    {
        public Guid Id { get; set; }
        public Guid AdvertisementId { get; set; }
        public Guid LocationId { get; set; }
    }
}