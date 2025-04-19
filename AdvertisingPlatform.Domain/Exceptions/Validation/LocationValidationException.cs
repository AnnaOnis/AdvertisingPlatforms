using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.Exceptions.Validation
{
    public class LocationValidationException : DomainValidationException
    {
        public LocationValidationException() { }

        public LocationValidationException(string message) : base(message) { }

        public LocationValidationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
