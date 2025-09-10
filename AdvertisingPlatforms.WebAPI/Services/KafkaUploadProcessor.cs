using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.DAL.Abstractions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Kafka.Abstractions;
using Confluent.Kafka;
using System.Diagnostics;
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
            _logger.LogInformation("Start process data from Kafka..." + DateTime.Now + $" Upload {batch.Count()} ");

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var payloads = batch.Select(b => b.Message.Value).ToList();
                var aggregated = "[" + string.Join(',', payloads) + "]";

                await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(aggregated));

                try
                {
                    _logger.LogInformation("Start upload data from Kafka..." + DateTime.Now);
                    await _uploadDataService.UploadDataFromStream(stream, ".json", UploadDataSources.KAFKA, cancellationToken);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.Commit();
                    _logger.LogInformation("Upload data from Kafka compleeted..." + DateTime.Now);
                }
                catch
                {
                    await _unitOfWork.RollBack();

                    throw;
                }                
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Обработка батча выполнена за {stopwatch.ElapsedMilliseconds} мс");
            }           
        }
    }
}


