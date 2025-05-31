using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Domain.Validators
{
    public class FileDataValidator : IFileDataValidator<IFileData>
    {
        public void Validate(IFileData? file)
        {
            if (file == null || file.FileLength == 0)
            {
                throw new DomainValidationException(ErrorMessages.FILE_IS_REQUIRED);
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension != FileConstants.ALLOWED_EXTENSION)
            {
                throw new DomainValidationException(ErrorMessages.ALLOWED_FILE_EXTENSION); ;
            }

            if (!file.ContentType.Equals(FileConstants.ALLOWED_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainValidationException(ErrorMessages.INVALID_FILE_TYPE); ;
            }
        }
    }
}
