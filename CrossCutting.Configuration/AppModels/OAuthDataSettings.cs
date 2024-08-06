namespace CrossCutting.Configuration.AppModels
{
    public class OAuthDataSettings
    {
        public string? ClientSecret { get { return GetClientSecret(); } }
        public string? ClientId { get { return GetClientId(); } }
        public string? TokenEndpoint { get { return GetTokenEndpoint(); } }
        public string? ApiEndpoint { get { return GetApiEndpoint(); } }
        public string? RedirectUri { get { return GetRedirectUri(); } }

        public string? TokenInfoEndpoint{ get { return GetTokenInfoEndpoint(); } }

        private static string? GetClientSecret()
        {
            return ConfigurationAppsettings.Builder()["GoogleOAuth:ClientId"];
        }
        private static string? GetClientId()
        {
            return ConfigurationAppsettings.Builder()["GoogleOAuth:ClientSecret"];
        }
        private static string? GetTokenEndpoint()
        {
            return ConfigurationAppsettings.Builder()["GoogleOAuth:TokenEndpoint"];
        }
        private static string? GetApiEndpoint()
        {
            return ConfigurationAppsettings.Builder()["GoogleOAuth:ApiEndpoint"];
        }
        private static string? GetRedirectUri()
        {
            return ConfigurationAppsettings.Builder()["GoogleOAuth:RedirectUri"];
        }
        private static string? GetTokenInfoEndpoint()
        {
            return ConfigurationAppsettings.Builder()["GoogleOAuth:TokenInfoEndpoint"];
        }
    }
}

