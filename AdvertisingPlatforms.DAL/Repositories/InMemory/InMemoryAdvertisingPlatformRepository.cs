using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Base.Extensions;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.Extensions.Logging;

namespace AdvertisingPlatforms.DAL.Repositories.InMemory
{
    public class InMemoryAdvertisingPlatformRepository :  IAdvertisingPlatformRepository
    {
        private readonly ConcurrentDictionary<Guid, AdvertisingPlatform> _platformsById = new();
        private readonly ConcurrentDictionary<string, HashSet<AdvertisingPlatform>> _platformsByLocationPrefix = new();
        private readonly ILogger<InMemoryAdvertisingPlatformRepository> _logger;

        public InMemoryAdvertisingPlatformRepository(ILogger<InMemoryAdvertisingPlatformRepository> logger)
        {
            _logger = logger;
        }

        public Task<AdvertisingPlatform> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!_platformsById.TryGetValue(id, out var entity))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + id);
            }

            return Task.FromResult(entity);
        }

        public Task<IReadOnlyCollection<AdvertisingPlatform>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(_platformsById.Values.ToList());
        }

        public async Task<AdvertisingPlatform> AddAsync(AdvertisingPlatform platform, CancellationToken cancellationToken)
        {
            if (!_platformsById.TryAdd(platform.Id, platform))
            {
                throw new EntityAlreadyExistsExeption(ErrorMessages.ENTITY_ALREADY_EXISTS + platform.Id);
            }

            await AddPlatformToLocationPrefixes(platform);
            return platform;
        }

        public async Task<IReadOnlyCollection<AdvertisingPlatform>> AddRangeAsync(IReadOnlyList<AdvertisingPlatform> platforms, CancellationToken cancellationToken)
        {
            var result = new List<AdvertisingPlatform>();
            foreach (var platform in platforms)
            {
                result.Add(await AddAsync(platform, cancellationToken));
            }

             return result;
        }

        public async Task UpdateAsync(AdvertisingPlatform platform, CancellationToken cancellationToken)
        {
            if (!_platformsById.TryGetValue(platform.Id, out var existingPlatform))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + platform.Id);
            }

            await RemovePlatformFromLocationPrefixes(existingPlatform);

            _platformsById[platform.Id] = platform;

            await AddPlatformToLocationPrefixes(platform);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (!_platformsById.TryRemove(id, out var platform))
            {
                throw new EntityNotFoundExeption(ErrorMessages.ENTITY_NOT_FOUND + id);
            }

            await RemovePlatformFromLocationPrefixes(platform);
        }

        public Task<IReadOnlyCollection<AdvertisingPlatform>> FindByLocationAsync(Location location, CancellationToken cancellationToken)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (_platformsByLocationPrefix.TryGetValue(location.Path, out var platforms))
            {
                return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(platforms);
            }

            return Task.FromResult<IReadOnlyCollection<AdvertisingPlatform>>(Array.Empty<AdvertisingPlatform>());
        }

        private Task AddPlatformToLocationPrefixes(AdvertisingPlatform platform)
        {
            foreach (var location in platform.Locations)
            {
                var prefixes = location.Path.GetPrefixes();
                foreach (var prefix in prefixes)
                {
                    _platformsByLocationPrefix.AddOrUpdate(
                        prefix,
                        _ => new HashSet<AdvertisingPlatform> { platform },
                        (_, set) =>
                        {
                            set.Add(platform);
                            return set;
                        });
                }
            }

            return Task.CompletedTask;
        }

        private Task RemovePlatformFromLocationPrefixes(AdvertisingPlatform platform)
        {
            foreach (var location in platform.Locations)
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
    }
}
