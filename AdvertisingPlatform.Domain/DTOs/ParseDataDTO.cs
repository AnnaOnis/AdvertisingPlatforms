
namespace AdvertisingPlatforms.Domain.DTOs
{
    public class ParseDataDto
    {
        public string AdvertisementName { get; set; }
        public IEnumerable<string> LocationPaths { get; set; }

        public ParseDataDto(string advertisementName, IEnumerable<string> locationPaths)
        {
            AdvertisementName = advertisementName;
            LocationPaths = locationPaths;
        }
    }
}
