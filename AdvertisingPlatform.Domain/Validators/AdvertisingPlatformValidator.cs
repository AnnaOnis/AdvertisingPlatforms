using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class AdvertisingPlatformValidator : IValidator<AdvertisingPlatform>
    {
        public void Validate(IEnumerable<AdvertisingPlatform>? platforms)
        {
            if (platforms == null)
                throw new ArgumentNullException(ErrorMessages.NULL_PLATFORMS_COLLECTION, nameof(platforms));

            if (!platforms.Any())
                throw new DomainValidationException(ErrorMessages.EMPTY_PLATFORMS_COLLECTION);

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
                throw new DomainValidationException(ErrorMessages.EMPTY_PLATFORM_NAME);

            if (!platform.Locations.Any())
                throw new DomainValidationException( ErrorMessages.EMPTY_LOCATIONS_COLLECTION_FOR_PLATFORM);
        }
    }
}
