
namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityAlreadyExistsExeption : Exception
    {
        public EntityAlreadyExistsExeption(string? message) : base(message)
        {
        }

        public EntityAlreadyExistsExeption(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}