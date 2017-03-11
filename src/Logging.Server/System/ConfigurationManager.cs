using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Specialized;

namespace System.Configuration
{
    public static class ConfigurationManager
    {
       // public static IConfigurationRoot Configuration { get; set; }

        public static void AddAppSettings(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfigurationRoot>();
           // Configuration = configuration;
            AppSettings = new NameValueCollection();
            foreach (var item in configuration.AsEnumerable())
            {
                AppSettings[item.Key] = item.Value;
            }
        }

        public static NameValueCollection AppSettings { get; set; }
    }
}