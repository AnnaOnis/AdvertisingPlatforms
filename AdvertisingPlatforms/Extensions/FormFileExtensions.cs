namespace AdvertisingPlatforms.Web.Extensions
{
    public static class FormFileExtensions
    {
        private const string allowedContentType = "text/plain";
        private const string allowedExtension = ".txt";
        public static bool IsValidTextFile(this IFormFile? file, out string errorMessage)
        {
            errorMessage = "";

            if (file == null || file.Length == 0)
            {
                errorMessage = "File is required";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != allowedExtension)
            {
                errorMessage = "Only .txt files are allowed";
                return false;
            }

            if (!file.ContentType.Equals(allowedContentType, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Invalid file type";
                return false;
            }

            return true;
        }
    }
}
