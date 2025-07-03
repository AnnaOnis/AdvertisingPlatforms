
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityNotFoundExeption : Exception
    {
        public EntityNotFoundExeption(string? message, Guid? entityId) : base(message)
        {
            message += $" id: {entityId}";
        }

        public EntityNotFoundExeption(string? message, Guid? entityId, Exception? innerException) : base(message, innerException)
        {
            message += $" id: {entityId}";
        }
    }
}