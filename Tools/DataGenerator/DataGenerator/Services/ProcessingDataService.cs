using DataGenerator.Abstractions;
using DataGenerator.Constants;
using DataGenerator.Models;
using System.Text.Json;

namespace DataGenerator.Services
{
    public class ProcessingDataService : IProcessingDataService
    {
        private JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        public string SerrializeToJson(List<Advertisement> advertisements)
        {
            string json = JsonSerializer.Serialize(advertisements, options);

            return json;
        }

        public async Task SaveToFile(string json)
        {
            var filePath = $"{ConfigConstants.DIRECTORI_PATH}/data_{DateTime.Now:dd.MM.yyyy_HHmmss}.json";
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
