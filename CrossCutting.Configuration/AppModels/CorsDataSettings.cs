using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Configuration.AppModels
{
    public class CorsDataSettings
    {
        public string? AllowedOrigins { get { return GetAllowedOrigins(); } }

        private static string? GetAllowedOrigins()
        {
            return ConfigurationAppsettings.Builder()["Cors:AllowedOrigins"];
        }
    }
}
