namespace AdvertisingPlatforms.Web.HttpModels.Responses
{
    public class LocationResponse
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
}
