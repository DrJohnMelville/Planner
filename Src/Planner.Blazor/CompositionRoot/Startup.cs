using System.Text.Json;
using Melville.MVVM.Time;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Blobs;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Repository.Web;

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
            services.AddSingleton<IWallClock, WallClock>();
            RegisterRepositories();
            SetupJsonSerialization();
        }

        private void SetupJsonSerialization()
        {
            var options = new JsonSerializerOptions();
            options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            options.IgnoreReadOnlyProperties = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            services.AddSingleton(options);
        }

        private void RegisterRepositories ()
        {
            services.AddTransient<IJsonWebService, JsonWebService>();
            services.AddSingleton<IBlobContentStore, WebBlobContentStore>();
            RegisterWebRepository<PlannerTask>("/Task");
            RegisterWebRepository<Note>("/Note");
            RegisterWebRepository<Blob>("/Blob");
            services.AddSingleton(typeof(ICachedRepositorySource<>), typeof(LocalToRemoteRepositoryBridge<>));
            services.AddSingleton(typeof(ILocalRepository<>), typeof(CachedRepository<>));
        }

        private void RegisterWebRepository<T>(string name) where T:PlannerItemWithDate
        {
            services.AddTransient<IDatedRemoteRepository<T>>
                (i => new WebRepository<T>(i.GetRequiredService<IJsonWebService>(), name));
        }
    }
}