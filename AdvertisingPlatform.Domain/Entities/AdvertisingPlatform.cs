using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Entities
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
        /// Name of the advertising platform
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of locations where the platform operates
        /// </summary>
        public IReadOnlyCollection<Location> Locations { get; set; }

        /// <summary>
        /// Creates a new instance of an advertising platform
        /// </summary>
        /// <param name="name">Platform name (cannot be empty)</param>
        /// <param name="locations">Array of locations (minimum one location required)</param>
        public AdvertisingPlatform( string name, IReadOnlyList<Location> locations) 
        {
            Id = Guid.NewGuid();
            Name = name;
            Locations = locations;
        }
    }
}
