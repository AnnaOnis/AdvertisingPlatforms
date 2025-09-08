
namespace AdvertisingPlatforms.Domain.DTOs
{
    public class ParsingResult
    {
        public List<ValidDataDto> ValidData { get; set; }
        public List<ErrorDataDto> Errors { get; set; }
        public bool IsSuccess => Errors == null || !Errors.Any();
        public TimeSpan ElapsedTime { get; set; }

        public ParsingResult()
        {
            ValidData = new List<ValidDataDto>();
            Errors = new List<ErrorDataDto>();
        }
    }
}
