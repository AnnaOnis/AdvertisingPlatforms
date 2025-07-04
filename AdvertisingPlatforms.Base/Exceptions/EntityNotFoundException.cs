using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.Base.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string? message, Guid? entityId) : base($"{message} id: {entityId}")
        {
        }

        public EntityNotFoundException(string? message, Guid? entityId, Exception? innerException) : base($"{message} id: {entityId}", innerException)
        {
        }
    }
}