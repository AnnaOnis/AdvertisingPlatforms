using System.Text;
using AdvertisingPlatforms.Base.Constants;

namespace AdvertisingPlatforms.DAL.Extensions
{
    public static class LocationStringExtensions
    {
        public static string NormalizeLocationPath(this string location)
        {
            if (string.IsNullOrEmpty(location)) return $"{TextSeparators.SLASH}";

            var trimmed = location.Trim().ToLowerInvariant();
            if (!trimmed.StartsWith(TextSeparators.SLASH)) trimmed = TextSeparators.SLASH + trimmed;
            return trimmed.TrimEnd(TextSeparators.SLASH);
        }

        public static IReadOnlyList<string> GetPrefixes(this string location)
        {
            if (string.IsNullOrEmpty(location)) throw new ArgumentNullException(nameof(location));

            location = location.Trim().ToLowerInvariant();
            var parts = location.Split(new[] { TextSeparators.SLASH }, StringSplitOptions.RemoveEmptyEntries);
            var prefixes = new List<string>();
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                sb.Append(TextSeparators.SLASH);
                sb.Append(part);
                prefixes.Add(sb.ToString());
            }

            return prefixes;
        }
    }
}
