using AdvertisingPlatforms.Base.Constants;
using AdvertisingPlatforms.Base.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace AdvertisingPlatforms.Base.Extensions
{
    public static class LocationStringExtensions
    {
        private static readonly Regex NotAllowedStrict = new(@"[^a-z/]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static string NormalizeLocationPath(this string location)
        {
            if (location is null)
                throw new DomainValidationException(ErrorMessages.NULL_LOCATION);

            var cleaned = location.Trim();

            // Единообразные слеши
            cleaned = cleaned.Replace('\\', TextSeparators.SLASH);

            // Убираем всё, что не разрешено
            cleaned = NotAllowedStrict.Replace(cleaned, "");

            // Сжать повторяющиеся слеши
            cleaned = Regex.Replace(cleaned, "/{2,}", "/");

            // Привести к нижнему регистру
            cleaned = cleaned.ToLowerInvariant();

            // Удалить конечный слеш, кроме корня
            if (cleaned.Length > 1 && cleaned.EndsWith(TextSeparators.SLASH))
                cleaned = cleaned.TrimEnd(TextSeparators.SLASH);

            // Гарантировать ведущий слеш
            if (!cleaned.StartsWith(TextSeparators.SLASH))
                cleaned = TextSeparators.SLASH + cleaned;

            // Пусто или только корень — считаем невалидным
            if (cleaned == "/" || cleaned.Length < 2)
                throw new DomainValidationException(ErrorMessages.EMPTY_LOCATION_PATH);

            return cleaned;
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
