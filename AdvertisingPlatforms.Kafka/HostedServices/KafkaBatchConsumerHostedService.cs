using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Kafka.HostedServices
{
    public class KafkaBatchConsumerHostedService : BackgroundService
    {
        private readonly ILogger<KafkaBatchConsumerHostedService> _logger;
        private readonly KafkaSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaBatchConsumerHostedService(IOptions<KafkaSettings> options,
            IServiceScopeFactory scopeFactory,
            ILogger<KafkaBatchConsumerHostedService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.ConsumerGroupId,
                SecurityProtocol = _settings.SecurityProtocol,
                SaslUsername = _settings.SaslUsername,
                SaslPassword = _settings.SaslPassword,
                SaslMechanism = _settings.SaslMechanism,
                AutoOffsetReset = _settings.AutoOffsetReset,
                EnableAutoCommit = false,
                SessionTimeoutMs = 6000,
                MaxPollIntervalMs = _settings.PollIntervalMs,
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(_settings.DataTopic);

            try
            {
                var batch = new List<ConsumeResult<string, string>>(_settings.MaxBatchSize);
                var lastCommit = DateTime.UtcNow;

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = await Task.Run(() =>
                            consumer.Consume(TimeSpan.FromMilliseconds(_settings.PollIntervalMs)),
                            stoppingToken);

                        if (cr == null) continue;

                        batch.Add(cr);

                        if (ShouldFlush(batch) || IsCommitIntervalExceeded(lastCommit))
                        {
                            await FlushAndCommitAsync(consumer, batch, stoppingToken);
                            lastCommit = DateTime.UtcNow;
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Игнорируем отмену
                    }
                }
            }
            finally
            {
                await FlushAndCommitAsync(consumer, new List<ConsumeResult<string, string>>(), stoppingToken);
                consumer.Close();
            }
        }

        private bool ShouldFlush(List<ConsumeResult<string, string>> batch)
            => batch.Count >= _settings.MaxBatchSize ||
               batch.Sum(x => (x.Message?.Key?.Length ?? 0) + (x.Message?.Value?.Length ?? 0)) >= _settings.MaxBatchBytes;

        private bool IsCommitIntervalExceeded(DateTime lastCommit)
            => (DateTime.UtcNow - lastCommit).TotalMilliseconds > _settings.CommitIntervalMs;

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
                _logger.LogError(ex, "Failed to process batch of {Count} messages", batch.Count);

                throw;
            }
        }
    }
}


