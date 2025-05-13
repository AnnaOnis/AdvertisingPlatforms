using AdvertisingPlatforms.Domain.Exceptions;

namespace AdvertisingPlatforms.Domain.Extensions
{
    public static class ValidationTextDataExtensions
    {
        public static void ValidateContentLine(this string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new DomainValidationException("Empty line");

            if (line.Count(c => c == ':') != 1)
                throw new DomainValidationException("Invalid format - missing colon separator");
        }
        public static void ValidatePlatformName(this string platformName)
        {
            if(string.IsNullOrWhiteSpace(platformName))
                throw new DomainValidationException("Platform name cannot be empty");
        }
        public static void ValidateLocation(this string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new DomainValidationException("Location cannot be empty");
        }
    }
}
