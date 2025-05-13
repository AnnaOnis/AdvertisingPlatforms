
namespace AdvertisingPlatforms.Domain.Entities
{
    public class Advertisement
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
