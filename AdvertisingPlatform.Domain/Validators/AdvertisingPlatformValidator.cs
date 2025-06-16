using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class AdvertisingPlatformValidator : IValidator<AdvertisingPlatformDb>
    {
        public void Validate(IEnumerable<AdvertisingPlatformDb>? platforms)
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
        public void Validate(AdvertisingPlatformDb? platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            if (string.IsNullOrWhiteSpace(platform.Advertisement.Name))
                throw new DomainValidationException(ErrorMessages.EMPTY_PLATFORM_NAME);
        }
    }
}
