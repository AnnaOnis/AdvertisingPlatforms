using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Domain.Abstractions;
using AdvertisingPlatforms.Domain.DTOs;

namespace AdvertisingPlatforms.Domain.Parser
{

    public class PlatformsFileParser : IAdvertisingPlatformParser
    {
        private readonly IEnumerable<IFileParseStrategy> _strategies;

        public PlatformsFileParser(IEnumerable<IFileParseStrategy> strategies)
        {
            _strategies = strategies;
        }

        public async Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> ParseFile(IFileData fileData, CancellationToken cancellationToken)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanParse(fileData));
            if (strategy == null)
                throw new DomainValidationException("Неподдерживаемый тип файла");
            return await strategy.Parse(fileData, cancellationToken);
        }

        public async Task<(List<ParseDataDto> Valid, List<UploadErrorDto> Errors)> ParseStream(Stream stream, string contentTypeOrExtension, CancellationToken cancellationToken)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanParse(contentTypeOrExtension));
            if (strategy == null)
                throw new DomainValidationException("Неподдерживаемый тип данных");
            return await strategy.Parse(stream, cancellationToken);
        }
    }
}
