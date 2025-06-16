using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.DTOs
{
    public class ParseDataDTO
    {
        public string AdvertisementName { get; set; }
        public IEnumerable<string> LocationPaths { get; set; }

        public ParseDataDTO(string advertisementName, IEnumerable<string> locationPaths)
        {
            AdvertisementName = advertisementName;
            LocationPaths = locationPaths;
        }
    }
}
