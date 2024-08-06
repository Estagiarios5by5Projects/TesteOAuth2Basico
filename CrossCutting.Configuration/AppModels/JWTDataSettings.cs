using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Configuration.AppModels
{
    public class JWTDataSettings
    {
        public string? Issuer { get { return GetIssuer(); } }
        public string? Audience { get { return GetAudience(); } }
        public string? SecretKey { get { return GetSecretKey(); } }

        private static string? GetIssuer()
        {
            return ConfigurationAppsettings.Builder()["ConnectionStrings:TesteOAuth2BasicoAPIContext"];
        }
        private static string? GetAudience()
        {
            return ConfigurationAppsettings.Builder()["ConnectionStrings:TesteOAuth2BasicoAPIContext"];
        }
        private static string? GetSecretKey()
        {
            return ConfigurationAppsettings.Builder()["ConnectionStrings:TesteOAuth2BasicoAPIContext"];
        }
    }
}
