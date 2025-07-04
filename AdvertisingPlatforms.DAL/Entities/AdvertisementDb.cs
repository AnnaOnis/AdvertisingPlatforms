
using System.ComponentModel.DataAnnotations;
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Entities
{
    public class AdvertisementDb : IEntity
    {
        public Guid Id { get; init; }

        [Required]
        public string Name { get; set; }

        public ICollection<AdvertisingPlatformDb> AdvertisingPlatforms { get; set; } = new HashSet<AdvertisingPlatformDb>();

        public AdvertisementDb(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        protected AdvertisementDb()
        {
        }
    }
}
