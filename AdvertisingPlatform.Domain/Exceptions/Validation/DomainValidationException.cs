namespace AdvertisingPlatforms.Domain.Exceptions.Validation
{
    [Serializable]
    public class DomainValidationException : DomainException
    {

        public DomainValidationException(string? message) : base(message, "VALIDATION_ERROR") { }

        public DomainValidationException(string? message, Exception? innerException) : base(message, innerException, "VALIDATION_ERROR") { }
    }
}