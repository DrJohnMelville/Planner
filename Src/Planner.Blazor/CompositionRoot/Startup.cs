using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;

namespace Planner.Blazor.CompositionRoot
{
    public class Startup
    {
        private readonly IServiceCollection services;

        public Startup(IServiceCollection services)
        {
            this.services = services;
        }

        public void Configure()
        {
            services.AddSingleton<IClock>(SystemClock.Instance);
        }
    }
}