using AdvertisingPlatforms.DAL.Entities;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class LocationValidator : IValidator<Location>
    {
        public void Validate(Location? location)
        {
            if (location == null)
                throw new ArgumentNullException( ErrorMessages.NULL_LOCATION, nameof(location));

            if (string.IsNullOrWhiteSpace(location.Path))
                throw new DomainValidationException(ErrorMessages.EMPTY_LOCATION_PATH);
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
