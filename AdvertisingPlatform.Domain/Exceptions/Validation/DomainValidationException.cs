namespace AdvertisingPlatforms.Domain.Exceptions.Validation
{
    [Serializable]
    public class DomainValidationException : Exception
    {
        public DomainValidationException() { }

        public DomainValidationException(string? message) : base(message) { }

        public DomainValidationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}