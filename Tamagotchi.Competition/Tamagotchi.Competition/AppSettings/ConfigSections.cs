
namespace Tamagotchi.Competition.AppSettings
{
    public static class ConfigSections
    {
        public const string DATABASE = "ConnectionStrings:LocalDB";
        public const string LOGGING = "Logging";
        public const string APP_CONFIG = "AppConfig";
        public const string SecretKey = "AppConfig:SecretKey";
        public const string CORS_POLICY = "AuthPolicy";
        public const string AUTH_BASE_URL = "AppConfig:AuthBaseUrl";
        public const string PROJECT_VERSION = "AppInfo:ProjectVersion";
        public const string COUNT_TOP_PLAYERS = "AppConfig:CountTopPlayers";
    }
}
