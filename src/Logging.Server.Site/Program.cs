using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Logging.Server.Site
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:88/server")
                .Build();

            //  var host = webHost.Build();
            ////  webHost.b
            //  webHost.UseUrls(Config.Listeners);

            //  host = webHost.UseUrls(Config.Listeners);// .Build();
            //  host.

            webHost.Run();
        }
    }
}