
using System.Text.RegularExpressions;

namespace AdvertisingPlatforms.Base.Extensions
{
    public static class AdvertisementStringExtensions
    {
        private static readonly Regex TrimStartNonAlphaNum = new(@"^[^\p{L}\p{Nd}]+", RegexOptions.Compiled);
        private static readonly Regex TrimEndNonAlphaNum = new(@"[^\p{L}\p{Nd}]+$", RegexOptions.Compiled);
        private static readonly Regex CollapseSpaces = new(@"\s{2,}", RegexOptions.Compiled);

        public static string NormalizeAdvertisementName(this string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var s = name.Trim();
            s = s.Trim('\"','\'','«','»','“','”','‘','’');
            s = TrimStartNonAlphaNum.Replace(s, "");
            s = TrimEndNonAlphaNum.Replace(s, "");
            s = CollapseSpaces.Replace(s, " ").Trim();
            return s;
        }
    }
}
