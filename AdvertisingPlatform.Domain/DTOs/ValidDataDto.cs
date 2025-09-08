
namespace AdvertisingPlatforms.Domain.DTOs
{
    public class ValidDataDto
    {
        public string AdvertisementName { get; set; }
        public IEnumerable<string> LocationPaths { get; set; }

        public ValidDataDto(string advertisementName, IEnumerable<string> locationPaths)
        {
            AdvertisementName = advertisementName;
            LocationPaths = locationPaths;
        }
    }
}
