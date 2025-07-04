using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Base.Extensions;
using System.ComponentModel.DataAnnotations;

namespace AdvertisingPlatforms.DAL.Entities
{
    /// <summary>
    /// Represents a location for an advertising platform
    /// </summary>
    public class LocationDb : IEntity
    {
        public Guid Id { get; init; }

        [Required]
        public string Path { get; set; }
        public Guid? ParentId { get; set; }

        public LocationDb? Parent { get; set; }
        public ICollection<LocationDb> Children { get; set; } = new List<LocationDb>();
        public ICollection<AdvertisingPlatformDb> AdvertisingPlatforms { get; set; } = new HashSet<AdvertisingPlatformDb>();


        public LocationDb(string path, Guid? parentId)
        {
            Id = Guid.NewGuid();
            Path = path;
            ParentId = parentId;
        }

        protected LocationDb()
        {
        }
    }
}
