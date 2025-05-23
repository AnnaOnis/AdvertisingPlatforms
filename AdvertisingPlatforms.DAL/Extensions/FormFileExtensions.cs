using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatforms.DAL.Extensions
{
    public static class FormFileExtensions
    {
        private const string _AllowedContentType = "text/plain";
        private const string _AllowedExtension = ".txt";

        public static bool IsValidTextFile(this IFormFile? file, out string errorMessage)
        {
            errorMessage = "";

            if (file == null || file.Length == 0)
            {
                errorMessage = "File is required";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != _AllowedExtension)
            {
                errorMessage = "Only .txt files are allowed";
                return false;
            }

            if (!file.ContentType.Equals(_AllowedContentType, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Invalid file type";
                return false;
            }

            return true;
        }
    }
}
