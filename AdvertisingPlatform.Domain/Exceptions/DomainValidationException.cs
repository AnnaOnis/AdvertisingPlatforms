namespace AdvertisingPlatforms.Domain.Exceptions
{
    [Serializable]
    internal class DomainValidationException : Exception
    {
        public DomainValidationException()
        {
        }

        public DomainValidationException(string? message) : base(message)
        {
        }

        public DomainValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}