using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Exceptions.Validation;
using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class AdvertisingPlatformValidator : IValidator<AdvertisingPlatform>
    {
        public void Validate(IEnumerable<AdvertisingPlatform>? platforms)
        {
            if (platforms == null)
                throw new ArgumentNullException(nameof(platforms));

            if (!platforms.Any())
                throw new PlatformValidationException("Collection of platforms cannot be empty");

            foreach (var platform in platforms)
            {
                Validate(platform);
            }
        }
        public void Validate(AdvertisingPlatform? platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            if (string.IsNullOrWhiteSpace(platform.Name))
                throw new PlatformValidationException($"Platform name cannot be empty (Platform: {platform.Name})");

            if (!platform.Locations.Any())
                throw new PlatformValidationException($"Platform must have at least one location (Platform: {platform.Name})");
        }
    }
}
