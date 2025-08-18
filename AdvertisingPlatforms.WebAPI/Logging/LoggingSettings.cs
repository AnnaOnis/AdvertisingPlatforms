namespace AdvertisingPlatforms.Web.Logging
{
    public class LoggingSettings
    {
        public bool EnableRequestLogging { get; set; } = true;
        public bool EnableResponseLogging { get; set; } = true;
        public int BodyLogLimit { get; set; } = 4096;
        public string[] SensitiveKeys { get; set; } = ["password", "token"];
        public string[] ExcludePaths { get; set; } = ["/swagger"];
        public bool IncludeHeaders { get; set; } = true;
    }
}
