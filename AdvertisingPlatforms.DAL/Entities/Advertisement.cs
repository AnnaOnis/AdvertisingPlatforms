
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Entities
{
    public class Advertisement : IEntity
    {
        /// <summary>
        /// Unique advertisement identifier
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Name of the advertisement 
        /// </summary>
        public string Name { get; set; }

        public Advertisement(string name) 
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
