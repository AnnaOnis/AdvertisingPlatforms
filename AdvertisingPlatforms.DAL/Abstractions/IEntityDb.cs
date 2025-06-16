namespace AdvertisingPlatforms.DAL.Abstractions
{
    /// <summary>
    /// Interface for all entities with unique identifier
    /// </summary>
    public interface IEntityDb
    {
        Guid Id { get; init; }
    }
}
