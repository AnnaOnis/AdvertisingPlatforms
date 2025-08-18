using DataGenerator.Abstractions;
using DataGenerator.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataGenerator.HostedServices
{
    public class ScheduledGenerationService : BackgroundService
    {

        private readonly IGenerationDataService _generator;
        private readonly ILogger<ScheduledGenerationService> _logger;

        public ScheduledGenerationService(
            IGenerationDataService generator,
            ILogger<ScheduledGenerationService> logger)
        {
            _generator = generator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(ConfigConstants.GENERATION_INTERVAL_MIN));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                _logger.LogInformation("Start of planned generation...");

                await _generator.GenerateAndSendToKafkaAsync(cancellationToken);

                _logger.LogInformation("Planned generation completed...");
            }
        }
    }
}


