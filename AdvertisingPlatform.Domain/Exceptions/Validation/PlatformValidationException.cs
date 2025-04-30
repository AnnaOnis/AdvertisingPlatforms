using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisingPlatforms.Domain.Exceptions.Validation
{
    public class PlatformValidationException : DomainValidationException
    {
        public PlatformValidationException(string? message) : base(message) { }

        public PlatformValidationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
