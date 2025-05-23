using AdvertisingPlatforms.Base.Constants;
using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.DAL.Extensions
{
    public static class FormFileExtensions
    {
        public static bool IsValidTextFile(this IFormFile? file, out string errorMessage)
        {
            errorMessage = "";

            if (file == null || file.Length == 0)
            {
                errorMessage = ErrorMessages.FILE_IS_REQUIRED;
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != FileConstants.ALLOWED_EXTENSION)
            {
                errorMessage = ErrorMessages.ALLOWED_FILE_EXTENSION;
                return false;
            }

            if (!file.ContentType.Equals(FileConstants.ALLOWED_CONTENT_TYPE, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = ErrorMessages.INVALID_FILE_TYPE;
                return false;
            }

            return true;
        }

        public static async Task<MemoryStream> FileToMemoryStreamAsync(this IFormFile file, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            return stream;
        }
    }
}
