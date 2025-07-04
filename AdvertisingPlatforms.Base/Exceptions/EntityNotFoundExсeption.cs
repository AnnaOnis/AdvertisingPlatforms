
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityNotFoundExсeption : Exception
    {
        public EntityNotFoundExсeption(string? message, Guid? entityId) : base(message)
        {
            message += $" id: {entityId}";
        }

        public EntityNotFoundExсeption(string? message, Guid? entityId, Exception? innerException) : base(message, innerException)
        {
            message += $" id: {entityId}";
        }
    }
}