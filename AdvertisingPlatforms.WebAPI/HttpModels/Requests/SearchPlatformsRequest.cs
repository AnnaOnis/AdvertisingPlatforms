namespace AdvertisingPlatforms.Web.HttpModels.Requests
{
    public class SearchPlatformsRequest
    {
        public string LocationPath { get; set; } = string.Empty;
        public string? SortBy { get; set; }
        public bool IsAsc { get; set; } = true;
    }
}