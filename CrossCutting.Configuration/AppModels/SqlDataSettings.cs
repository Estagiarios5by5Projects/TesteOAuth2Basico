using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Configuration.AppModels
{
    public  class SqlDataSettings
    {
        public  string? ConnectionString { get { return GetConnectionString(); } }

        private static string? GetConnectionString()
        {
           return ConfigurationAppsettings.Builder()["ConnectionStrings:TesteOAuth2BasicoAPIContext"];
        }
    }
}
