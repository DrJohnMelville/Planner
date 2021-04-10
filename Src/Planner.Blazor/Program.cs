using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Planner.Blazor.CompositionRoot;

namespace Planner.Blazor
{
    #warning try publishing an untrimmed app once the production framework is out/
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            new Startup(builder.Services, builder.HostEnvironment).Configure();
            
            await builder.Build().RunAsync();
        }
    }
}
