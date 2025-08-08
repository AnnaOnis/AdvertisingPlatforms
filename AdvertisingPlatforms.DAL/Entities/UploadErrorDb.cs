
using AdvertisingPlatforms.DAL.Abstractions;

namespace AdvertisingPlatforms.DAL.Entities
{
    public class UploadErrorDb : IEntity
    {
        public Guid Id { get; init; }
        public string Source { get; set; }
        public string RawData { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }

        public UploadErrorDb(string source, string rawData, string errorType, string errorMessage)
        {
            Id = Guid.NewGuid();
            Source = source;
            RawData = rawData;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
            Timestamp = DateTime.UtcNow;
        }
    }
}
