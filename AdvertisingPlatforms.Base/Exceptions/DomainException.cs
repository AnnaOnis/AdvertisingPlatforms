using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        public string Type { get; } = ExceptionTypes.DOMAIN_ERROR;

        public DomainException(string? message) : base(message) { }

        protected DomainException(string? message, string type) : base(message) 
        { 
            Type = type;
        }
        public DomainException(string? message, Exception? innerException, string type) : base(message, innerException)
        {
            Type = type;
        }
    }
}
