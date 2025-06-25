using AdvertisingPlatforms.Domain.Models;

namespace AdvertisingPlatforms.Domain.Abstractions
{
    /// <summary>
    /// Service for working with advertisements
    /// </summary>
    public interface IAdvertisementService
    {
        /// <summary>
        /// Get advertisement by Id
        /// </summary>
        /// <param name="advertisementId">Advertisement Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested advertisement.</returns>
        Task<Advertisement> GetById(Guid advertisementId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all advertisements
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The collection of advertisements.</returns>
        Task<IReadOnlyCollection<Advertisement>> GetAllAdvertisements(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new advertisement
        /// </summary>
        /// <param name="advertisement">The advertisement to be added.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created advertisement with assigned ID.</returns>
        Task<Advertisement> CreateAdvertisement(string advertisementName, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing advertisement
        /// </summary>
        /// <param name="advertisement">The advertisement to be updated.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task UpdateAdvertisement(Guid advertisemntId, string newAdvertisementName, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an advertisement
        /// </summary>
        /// <param name="advertisementId">ID of the advertisement to delete.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task DeleteAdvertisement(Guid advertisementId, CancellationToken cancellationToken);

        /// <summary>
        /// Find advertisement by name
        /// </summary>
        /// <param name="name">Advertisement name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The advertisement if found, null otherwise.</returns>
        Task<Advertisement?> FindByName(string name, CancellationToken cancellationToken);
    }
} 