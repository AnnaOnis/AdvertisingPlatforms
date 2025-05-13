namespace AdvertisingPlatforms.DAL.Abstractions
{
    /// <summary>
    /// Interface for all entities with unique identifier
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; init; }
    }
}
