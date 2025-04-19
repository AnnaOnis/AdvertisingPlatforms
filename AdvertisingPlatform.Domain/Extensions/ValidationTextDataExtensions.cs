using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Domain.Exceptions.Validation;

namespace AdvertisingPlatforms.Domain.Extensions
{
    public static class ValidationTextDataExtensions
    {
        public static void ValidateContentLine(this string line, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new LineValidationException(lineNumber, "Empty line");

            if (line.Count(c => c == ':') != 1)
                throw new LineValidationException(lineNumber, "Invalid format - missing colon separator");
        }

        public static void ValidatePlatformName(this string platformName, int lineNumber)
        {
            if(string.IsNullOrWhiteSpace(platformName))
                throw new LineValidationException(lineNumber, "Platform name cannot be empty");
        }

        public static void ValidateLocation(this string location, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new LineValidationException(lineNumber, "Location cannot be empty");
        }
    }
}
