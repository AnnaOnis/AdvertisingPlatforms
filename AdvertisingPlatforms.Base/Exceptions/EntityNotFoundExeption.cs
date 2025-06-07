
namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityNotFoundExeption : Exception
    {
        public EntityNotFoundExeption(string? message) : base(message)
        {
        }

        public EntityNotFoundExeption(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}