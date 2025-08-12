
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Domain.Models
{
    public class AdvertisingPlatform : IDomainModel
    {
        public Guid Id { get; init; }
        public Guid AdvertisementId { get; init; }
        public Guid LocationId { get; init; }
        public string AdvertisementName { get; init; }
        public string LocationPath { get; init; }

        public AdvertisingPlatform(Guid id, 
            Guid advertisementId, 
            Guid locationId,
            string advertisementName,
            string locationPath)
        {
            Id = id;
            AdvertisementId = advertisementId;
            LocationId = locationId;
            AdvertisementName = advertisementName;
            LocationPath = locationPath;
        }
    }
}
