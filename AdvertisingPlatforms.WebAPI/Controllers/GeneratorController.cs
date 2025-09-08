using AdvertisingPlatforms.Kafka.Abstractions;
using AdvertisingPlatforms.Kafka.Models;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

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

        [HttpPost]
        public async Task<IActionResult> TriggerGeneration(int countData, CancellationToken cancellationToken)
        {

            await _producer.ProduceAsync(
                topic: _settings.ComandTopic,
                key: "manual-trigger",
                value: countData.ToString(),
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Generation trigger sended...");

            return Ok();
        }
    }
}



