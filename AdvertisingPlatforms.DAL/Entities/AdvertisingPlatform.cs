using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Entities
{
    /// <summary>
    /// Represents an advertising platform with a set of locations
    /// </summary>
    public class AdvertisingPlatform : IEntity
    {
        /// <summary>
        /// Unique platform identifier
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Advertisement of the platform
        /// </summary>
        public Advertisement Advertisement { get; set; }

        /// <summary>
        /// Collection of locations where the platform operates
        /// </summary>
        public IReadOnlyCollection<Location> Locations { get; set; }

        /// <summary>
        /// Creates a new instance of an advertising platform
        /// </summary>
        /// <param name="name">Platform name (cannot be empty)</param>
        /// <param name="locations">Array of locations (minimum one location required)</param>
        public AdvertisingPlatform(Advertisement advertisement, IReadOnlyList<Location> locations) 
        {
            Id = Guid.NewGuid();
            Advertisement = advertisement;
            Locations = locations;
        }
    }
}
