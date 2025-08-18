using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdvertisingPlatforms.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratorController : ControllerBase
    {
        private readonly IKafkaProducer _producer;
        private readonly KafkaSettings _settings;
        private readonly ILogger<GeneratorController> _logger;

        public GeneratorController(
            IKafkaProducer producer,
            IOptions<KafkaSettings> options,
            ILogger<GeneratorController> logger)
        {
            _producer = producer;
            _settings = options.Value;
            _logger = logger;
        }

        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerGeneration(CancellationToken ct)
        {
            
            await _producer.ProduceAsync(
                topic: _settings.ComandTopic,
                key: "manual-trigger",
                value: DateTime.UtcNow.ToString("O"),
                cancellationToken: ct
            );

            _logger.LogInformation("Generation trigger sended...");

            return Ok();
        }
    }
}



