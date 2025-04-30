using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Extensions;
using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Entities
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
