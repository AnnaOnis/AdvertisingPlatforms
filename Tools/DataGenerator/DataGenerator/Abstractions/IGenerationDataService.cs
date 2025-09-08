using DataGenerator.Models;

namespace DataGenerator.Abstractions
{
    public interface IGenerationDataService
    {
        List<Advertisement> GenerateTestData(int count);
    }
}
