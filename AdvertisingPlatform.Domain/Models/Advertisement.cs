
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Domain.Models
{
    public class Advertisement : IDomainModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; }

        public Advertisement(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
