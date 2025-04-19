using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Exceptions;
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
                throw new DomainValidationException("Collection cannot be empty");

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
                throw new DomainValidationException($"Platform name cannot be empty (Platform: {platform.Name})");

            if (!platform.Locations.Any())
                throw new DomainValidationException($"Platform must have at least one location (Platform: {platform.Name})");
        }
    }
}
