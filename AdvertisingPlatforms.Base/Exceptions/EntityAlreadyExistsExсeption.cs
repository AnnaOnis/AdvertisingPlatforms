
namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityAlreadyExistsExсeption : Exception
    {
        public EntityAlreadyExistsExсeption(string? message) : base(message)
        {
        }

        public EntityAlreadyExistsExсeption(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}