using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Entities
{
    /// <summary>
    /// Represents an advertising platform with a set of locations
    /// </summary>
    public class AdvertisingPlatformDb : IEntity
    {
        public Guid Id { get; init; }
        public Guid AdvertisementId { get; set; }
        public Guid LocationId { get; set; }


        public AdvertisementDb Advertisement { get; set; } = null!;
        public LocationDb Location { get; set; } = null!;

        public AdvertisingPlatformDb(Guid advertisementId, Guid locationId)
        {
            Id = Guid.NewGuid();
            AdvertisementId = advertisementId;
            LocationId = locationId;
        }

        protected AdvertisingPlatformDb()
        {
        }
    }
}
