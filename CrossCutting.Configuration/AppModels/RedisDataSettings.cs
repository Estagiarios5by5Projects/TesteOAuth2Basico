using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Configuration.AppModels
{
    public class RedisDataSettings
    {
        public string? ConnectionStringRedis { get { return GetConnectionStringRedis(); } }

        private static string? GetConnectionStringRedis()
        {
            return ConfigurationAppsettings.Builder()["Redis:ConnectionString"];
        }

    }
}
