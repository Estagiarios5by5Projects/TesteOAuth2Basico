using Microsoft.Extensions.Configuration;

namespace CrossCutting.Configuration
{
    internal static class ConfigurationAppsettings
    {
        internal static IConfigurationRoot Builder()
        {
           var builder = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
