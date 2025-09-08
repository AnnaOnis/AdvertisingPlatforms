using DataGenerator.Models;

namespace DataGenerator.Abstractions
{
    public interface IProcessingDataService
    {
        string SerrializeToJson(List<Advertisement> advertisements);
        Task SaveToFile(string json);
        Task SendDataToKafkaAsync(List<Advertisement> advertisements, CancellationToken cancellationToken);

    }
}
