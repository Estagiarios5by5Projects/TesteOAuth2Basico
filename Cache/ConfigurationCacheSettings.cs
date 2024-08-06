using Microsoft.Extensions.Configuration;

namespace Cache
{
    internal static class ConfigurationCacheSettings
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
