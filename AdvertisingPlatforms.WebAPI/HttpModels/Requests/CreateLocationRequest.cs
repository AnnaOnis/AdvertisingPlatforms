namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class CreateLocationRequest
    {
        public string Path { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
} 