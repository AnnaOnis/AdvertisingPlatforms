using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace AdvertisingPlatforms.Web.Services
{
    public class KafkaUploadProcessor : IBatchKafkaConsumerProcessor
    {
        private readonly IUploadDataService _uploadDataService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<KafkaUploadProcessor> _logger;

        public KafkaUploadProcessor(IUploadDataService uploadDataService, IUnitOfWork unitOfWork, ILogger<KafkaUploadProcessor> logger)
        {
            _uploadDataService = uploadDataService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ProcessBatchAsync(IEnumerable<ConsumeResult<string, string>> batch, CancellationToken cancellationToken)
        {
            var payloads = batch.Select(b => b.Message.Value).ToList();
            var aggregated = "[" + string.Join(',', payloads) + "]";

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(aggregated));

            try
            {
                await _uploadDataService.UploadDataFromStream(stream, ".json", UploadDataSources.KAFKA, cancellationToken);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.Commit();
            }
            catch
            {
                try
                {
                    await _unitOfWork.RollBack();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Rollback failed in KafkaUploadProcessor");
                    throw;
                }
                throw;
            }
        }
    }
}


