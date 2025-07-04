
namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException(string? message) : base(message)
        {
        }

        public EntityAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}