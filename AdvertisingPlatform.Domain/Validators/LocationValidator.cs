using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Exceptions;
using AdvertisingPlatforms.Domain.Interfaces;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class LocationValidator : IValidator<Location>
    {
        public void Validate(Location? location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (string.IsNullOrWhiteSpace(location.Path))
                throw new DomainValidationException($"Location path cannot be empty");
        }

        public void Validate(IEnumerable<Location>? locations)
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));

            foreach (var location in locations)
            {
                Validate(location);
            }
        }
    }
}
