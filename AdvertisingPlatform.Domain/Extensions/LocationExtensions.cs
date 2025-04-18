using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertisingPlatforms.Services;

namespace AdvertisingPlatforms.Domain.Extensions
{
    public static class LocationExtensions
    {
        private const char slash = '/';
        public static string NormalizeLocation(this string location)
        {
            if (string.IsNullOrEmpty(location)) return $"{slash}";

            var trimmed = location.Trim();
            if (!trimmed.StartsWith(slash)) trimmed = slash + trimmed;
            return trimmed.TrimEnd(slash);
        }
        public static IReadOnlyList<string> GetPrefixes(this string location)
        {
            if (string.IsNullOrEmpty(location)) throw new ArgumentNullException(nameof(location));

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
