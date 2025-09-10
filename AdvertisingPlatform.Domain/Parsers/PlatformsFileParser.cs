using AdvertisingPlatforms.Base.Constants;
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

        public async Task<ParsingResult> ParseFile(IFileData fileData, CancellationToken cancellationToken)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanParse(fileData));

            return strategy == null
              ? throw new DomainValidationException(ErrorMessages.INVALID_FILE_TYPE)
              : await strategy.Parse(fileData, cancellationToken);

        }

        public async Task<ParsingResult> ParseStream(Stream stream, string contentTypeOrExtension, CancellationToken cancellationToken)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanParse(contentTypeOrExtension));
            return strategy == null
              ? throw new DomainValidationException(ErrorMessages.INVALID_FILE_TYPE)
              : await strategy.Parse(stream, cancellationToken);
        }
    }
}
