namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class CreateAdvertisingPlatformRequest
    {
        public Guid AdvertisementId { get; set; }
        public Guid LocationId { get; set; }
    }
}