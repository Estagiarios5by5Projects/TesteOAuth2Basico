using CrossCutting.Configuration.AppModels;

namespace CrossCutting.Configuration
{
    public static class AppSettings
    {
        public static SqlDataSettings SqlDataSettings = new SqlDataSettings();
        public static OAuthDataSettings OAuthDataSettings = new OAuthDataSettings();
        public static JWTDataSettings JWTDataSettings = new JWTDataSettings();
        public static CorsDataSettings CorsDataSettings = new CorsDataSettings();
        public static RedisDataSettings RedisDataSettings = new RedisDataSettings();
    }
}
