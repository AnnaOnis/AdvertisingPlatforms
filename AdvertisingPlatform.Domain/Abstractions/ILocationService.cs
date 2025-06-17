using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    /// <summary>
    /// Service for working with locations
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Get location by Id
        /// </summary>
        /// <param name="locationId">Location Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested location.</returns>
        Task<Location> GetById(Guid locationId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all locations
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The collection of locations.</returns>
        Task<IReadOnlyCollection<Location>> GetAllLocations(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new location
        /// </summary>
        /// <param name="location">The location to be added.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created location with assigned ID.</returns>
        Task<Location> AddLocation(Location location, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing location
        /// </summary>
        /// <param name="location">The location to be updated.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task UpdateLocation(Location location, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a location
        /// </summary>
        /// <param name="locationId">ID of the location to delete.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task DeleteLocation(Guid locationId, CancellationToken cancellationToken);

        /// <summary>
        /// Find location by path
        /// </summary>
        /// <param name="path">Location path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The location if found, null otherwise.</returns>
        Task<Location?> FindByPath(string path, CancellationToken cancellationToken);
    }
} 