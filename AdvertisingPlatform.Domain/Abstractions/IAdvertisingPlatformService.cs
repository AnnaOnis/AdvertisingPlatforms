using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    /// <summary>
    /// Service for working with advertising platforms
    /// </summary>
    public interface IAdvertisingPlatformService
    {
        /// <summary>
        /// Get platform by Id
        /// </summary>
        /// <param name="platformId">Platform Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested platform.</returns>
        Task<AdvertisingPlatform> GetById(Guid platformId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all platform
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The collection of platforms.</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> GetAllPlatforms(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new platform
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="platform">The platform to be added.</param>
        /// <returns>The added platform.</returns>
        Task AddPlatform(AdvertisingPlatform platform, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing platform
        /// </summary>
        /// <param name="platform">The platform to be updated.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task UpdatePlatform(AdvertisingPlatform platform, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a platform
        /// </summary>
        /// <param name="platformId">ID of the platform to delete.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task DeletePlatform(Guid platformId, CancellationToken cancellationToken);

        /// <summary>
        /// Searches for platforms by specified location
        /// </summary>
        /// <param name="location">Target location</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of matching platforms</returns>
        Task<IReadOnlyCollection<AdvertisingPlatform>> Search(string locationPath,
            CancellationToken cancellationToken,
            string? sortBy = null,
            bool isAsc = true);
    }
}
