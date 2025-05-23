using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Extensions;

namespace AdvertisingPlatforms.DAL.Entities
{
    /// <summary>
    /// Represents a location for an advertising platform
    /// </summary>
    public class Location : IEntity
    {
        /// <summary>
        /// Unique location identifier
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Location path
        /// </summary>
        public string Path { get; init; }

        /// <summary>
        /// Creates a new location instance
        /// </summary>
        /// <param name="path">Location path (cannot be empty)</param>
        /// <exception cref="ArgumentException">Thrown for invalid arguments</exception>
        public Location(string path)
        {
            Id = Guid.NewGuid();
            Path = path.NormalizeLocationPath();
        }
    }
}
