namespace Cache
{
    public class RedisDataSettings
    {
        public string? ConnectionStringRedis { get { return GetConnectionStringRedis(); } }

        private static string? GetConnectionStringRedis()
        {
            return ConfigurationCacheSettings.Builder()["Redis:ConnectionString"];
        }

    }
}
