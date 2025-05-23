using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Domain.Exceptions;
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class AdvertisingPlatformValidator : IValidator<AdvertisingPlatform>
    {
        public void Validate(IEnumerable<AdvertisingPlatform>? platforms)
        {
            if (platforms == null)
                throw new ArgumentNullException(nameof(platforms));

            if (!platforms.Any())
                throw new DomainValidationException("Collection of platforms cannot be empty");

            foreach (var platform in platforms)
            {
                Validate(platform);
            }
        }
        public void Validate(AdvertisingPlatform? platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            if (string.IsNullOrWhiteSpace(platform.Advertisement.Name))
                throw new DomainValidationException($"Platform name cannot be empty (Platform: {platform.Advertisement.Name})");

            if (!platform.Locations.Any())
                throw new DomainValidationException($"Platform must have at least one location (Platform: {platform.Advertisement.Name})");
        }
    }
}
