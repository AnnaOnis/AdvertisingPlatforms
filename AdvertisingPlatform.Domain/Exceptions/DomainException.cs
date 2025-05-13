namespace AdvertisingPlatforms.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Type { get; } = "DOMAIN_ERROR";
        public DomainException(string? message, string type) : base(message) 
        { 
            Type = type;
        }
        public DomainException(string? message, Exception? innerException, string type) : base(message, innerException)
        {
            Type = type;
        }
    }
}
