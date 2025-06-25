namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class UpdateLocationRequest
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
} 