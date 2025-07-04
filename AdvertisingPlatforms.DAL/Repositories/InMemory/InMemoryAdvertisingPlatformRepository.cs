using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Delegates;
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.Extensions.Logging;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class InMemoryAdvertisingPlatformRepository : InMemoryRepository<AdvertisingPlatformDb>, IAdvertisingPlatformRepository
    {
        private readonly ConcurrentDictionary<string, HashSet<AdvertisingPlatformDb>> _platformsByLocationPrefix = new();
        public InMemoryAdvertisingPlatformRepository()
        {
        }

        public override async Task<AdvertisingPlatformDb> AddAsync(AdvertisingPlatformDb platform, CancellationToken cancellationToken)
        {
            await base.AddAsync(platform, cancellationToken);
            await AddPlatformToLocationPrefixes(platform);
            return platform;
        }

        public override async Task<IReadOnlyCollection<AdvertisingPlatformDb>> AddRangeAsync(IReadOnlyList<AdvertisingPlatformDb> platforms, CancellationToken cancellationToken)
        {
            var result = new List<AdvertisingPlatformDb>();
            foreach (var platform in platforms)
            {
                result.Add(await AddAsync(platform, cancellationToken));
            }         

             return result;
        }

        public override async Task UpdateAsync(AdvertisingPlatformDb platform, CancellationToken cancellationToken)
        {
            if (!_entityById.TryGetValue(platform.Id, out var existingPlatform))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, platform.Id);
            }

            await RemovePlatformFromLocationPrefixes(existingPlatform);

            _entityById[platform.Id] = platform;

            await AddPlatformToLocationPrefixes(platform);
        }

        public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!_entityById.TryRemove(id, out var platform))
            {
                throw new EntityNotFoundException(ErrorMessages.ENTITY_NOT_FOUND, id);
            }

            await RemovePlatformFromLocationPrefixes(platform);
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatformDb>> FindByLocationAsync(LocationDb location,
            CancellationToken cancellationToken, 
            AdvertisingPlatformsSortDelegate? sortDelegate = null)
        {
            if (!_platformsByLocationPrefix.TryGetValue(location.Path, out var platforms))
            {
                return await Task.FromResult(Array.Empty<AdvertisingPlatformDb>());
            }
            var query = platforms.AsQueryable();

            if (sortDelegate != null)
            {
                query = sortDelegate(query);
            }

            return await Task.FromResult(query.ToList());
        }

        private Task AddPlatformToLocationPrefixes(AdvertisingPlatformDb platform)
        {
           var location = platform.Location;
            if (location != null)
            {
                var prefixes = location.Path.GetPrefixes();
                foreach (var prefix in prefixes)
                {
                    _platformsByLocationPrefix.AddOrUpdate(
                        prefix,
                        _ => new HashSet<AdvertisingPlatformDb> { platform },
                        (_, set) =>
                        {
                            set.Add(platform);
                            return set;
                        });
                }
            }

            return Task.CompletedTask;
        }

        private Task RemovePlatformFromLocationPrefixes(AdvertisingPlatformDb platform)
        {
            var location = platform.Location;
            if (location != null)
            {
                var prefixes = location.Path.GetPrefixes();
                foreach (var prefix in prefixes)
                {
                    if (_platformsByLocationPrefix.TryGetValue(prefix, out var platforms))
                    {
                        platforms.Remove(platform);
                        if (platforms.Count == 0)
                        {
                            _platformsByLocationPrefix.TryRemove(prefix, out _);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByAdvertisementAndLocationAsync(Guid advertisementId, Guid locationId, CancellationToken cancellationToken)
        {
            return _entityById.Values.Any(p => p.AdvertisementId == advertisementId && p.LocationId == locationId);
        }
    }
}
