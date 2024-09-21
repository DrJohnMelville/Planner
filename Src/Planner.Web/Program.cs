using Melville.IOC.AspNet.RegisterFromServiceCollection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Planner.Web.CompositionRoot;
using Serilog;

namespace Planner.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new MelvilleServiceProviderFactory(true))
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}