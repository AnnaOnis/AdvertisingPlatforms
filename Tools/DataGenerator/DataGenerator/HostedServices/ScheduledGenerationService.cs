using DataGenerator.Abstractions;
using DataGenerator.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataGenerator.HostedServices
{
    public class ScheduledGenerationService : BackgroundService
    {

        private readonly IGenerationDataService _generator;
        private readonly IProcessingDataService _processingDataService;
        private readonly ILogger<ScheduledGenerationService> _logger;

        public ScheduledGenerationService(
            IGenerationDataService generator,
            IProcessingDataService processingDataService,
            ILogger<ScheduledGenerationService> logger)
        {
            _generator = generator;
            _processingDataService = processingDataService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(ConfigConstants.GENERATION_INTERVAL_MIN));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Start of planned generation...");

                var random = new Random();
                var randomCount = random.Next(ConfigConstants.MIN_ADS, ConfigConstants.MAX_ADS);

                var data = _generator.GenerateTestData(randomCount);

                await _processingDataService.SendDataToKafkaAsync(data, stoppingToken);

                _logger.LogInformation("Planned generation completed...");
            }
        }
    }
}


