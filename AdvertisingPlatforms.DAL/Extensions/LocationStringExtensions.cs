using System.Text;

namespace AdvertisingPlatforms.Domain.Extensions
{
    public static class LocationStringExtensions
    {
        private const char slash = '/';

        public static string NormalizeLocationPath(this string location)
        {
            if (string.IsNullOrEmpty(location)) return $"{slash}";

            var trimmed = location.Trim().ToLowerInvariant();
            if (!trimmed.StartsWith(slash)) trimmed = slash + trimmed;
            return trimmed.TrimEnd(slash);
        }

        public static IReadOnlyList<string> GetPrefixes(this string location)
        {
            if (string.IsNullOrEmpty(location)) throw new ArgumentNullException(nameof(location));

            location = location.Trim().ToLowerInvariant();
            var parts = location.Split(new[] { slash }, StringSplitOptions.RemoveEmptyEntries);
            var prefixes = new List<string>();
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                sb.Append(slash);
                sb.Append(part);
                prefixes.Add(sb.ToString());
            }

            return prefixes;
        }
    }
}
