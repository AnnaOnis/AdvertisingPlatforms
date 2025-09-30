using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Kafka.HostedServices
{
    public class KafkaBatchConsumerHostedService : KafkaConsumerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaBatchConsumerHostedService(
            ILogger<KafkaBatchConsumerHostedService> logger,
            IOptions<KafkaSettings> options,
            IServiceScopeFactory scopeFactory)
            : base(logger, options, options.Value.DataTopic)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ConsumeMessagesAsync(IConsumer<string, string> consumer, CancellationToken stoppingToken)
        {
            var batch = new List<ConsumeResult<string, string>>(Settings.MaxBatchSize);
            var lastCommitTime = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = await ConsumeMessageAsync(consumer, stoppingToken);

                if (consumeResult != null)
                {
                    batch.Add(consumeResult);
                }

                if (ShouldFlush(batch) || IsCommitIntervalExceeded(lastCommitTime))
                {
                    await FlushAndCommitAsync(consumer, batch, stoppingToken);
                    lastCommitTime = DateTime.UtcNow;
                }
            }
        }

        private bool ShouldFlush(List<ConsumeResult<string, string>> batch)
            => batch.Count >= Settings.MaxBatchSize ||
               batch.Sum(x => (x.Message?.Key?.Length ?? 0) + (x.Message?.Value?.Length ?? 0)) >= Settings.MaxBatchBytes;

        private bool IsCommitIntervalExceeded(DateTime lastCommitTime)
            => (DateTime.UtcNow - lastCommitTime).TotalMilliseconds > Settings.CommitIntervalMs;

        private async Task FlushAndCommitAsync(IConsumer<string, string> consumer, List<ConsumeResult<string, string>> batch, CancellationToken token)
        {
            if (batch.Count == 0) return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IBatchKafkaConsumerProcessor>();
                await processor.ProcessBatchAsync(batch, token);

                consumer.Commit(batch.Select(x => x.TopicPartitionOffset));
                batch.Clear();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process batch of {Count} messages", batch.Count);
                throw;
            }
        }
    }
}


