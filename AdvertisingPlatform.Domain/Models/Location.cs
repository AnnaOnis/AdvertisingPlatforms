
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Domain.Models
{
    public class Location : IDomainModel
    {
        public Guid Id { get; init; }
        public string Path { get; init; }
        public Guid? ParentId { get; init; }

        public Location(Guid id, string path, Guid? parentId)
        {
            Id = id;
            Path = path.NormalizeLocationPath();
            ParentId = parentId;
        }
    }
}
