using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class DomainValidationException : DomainException
    {

        public DomainValidationException(string? message) : base(message, ExceptionTypes.VALIDATION_ERROR) { }

        public DomainValidationException(string? message, Exception? innerException) : base(message, innerException, ExceptionTypes.VALIDATION_ERROR) { }
    }
}