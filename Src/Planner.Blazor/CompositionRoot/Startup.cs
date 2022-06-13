using System.Text.Json;
using System.Text.Json.Serialization;
using Melville.SystemInterface.Time;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Blazor.ModalComponent;
using Planner.Blazor.Pages;
using Planner.Models.Blobs;
using Planner.Models.HtmlGeneration;
using Planner.Models.Markdown;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;
using Planner.Repository.Web;
using Tewr.Blazor.FileReader;

namespace Planner.Blazor.CompositionRoot
{
    public class Startup
    {
        private readonly IServiceCollection services;
        private readonly IWebAssemblyHostEnvironment hostEnvironment;

        public Startup(IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
        {
            this.services = services;
            this.hostEnvironment = hostEnvironment;
        }

        public void Configure()
        {
            RegisterModalComponents();
            RegisterClocks();
            RegisterRepositories();
            RegisterNavigation();
            SetupJsonSerialization();
            RegisterNoteRendering();
        }

        private void RegisterNoteRendering()
        {
            services.AddSingleton<IMarkdownTranslator>(
                new MarkdownTranslator("/DailyPage/", hostEnvironment.BaseAddress+"BlobContent/"));
            services.AddFileReaderService(o => o.UseWasmSharedBuffer = true);
            services.AddTransient<IBlobCreator, BlobCreator>();
        }

        private void RegisterNavigation()
        {
            services.AddTransient<IAppNavigation, AppNavigation>();
        }

        private void RegisterModalComponents()
        {
            services.AddScoped<ModalService>();
        }

        private void RegisterClocks()
        {
            services.AddSingleton<IClock>(SystemClock.Instance);
            services.AddSingleton<IWallClock, WallClock>();
            services.AddSingleton<IUsersClock, UsersClock>();
        }

        private void SetupJsonSerialization()
        {
            var options = new JsonSerializerOptions();
            options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            options.IgnoreReadOnlyProperties = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.ReferenceHandler = ReferenceHandler.Preserve;
            services.AddSingleton(options);
        }

        private void RegisterRepositories ()
        {
            services.AddSingleton(typeof(IEventBroadcast<>), typeof(EventBroadcast<>));
            services.AddTransient<IJsonWebService, JsonWebService>();
            services.AddSingleton<IBlobContentStore, WebBlobContentStore>();
            services.AddTransient<INoteSearcher, WebNoteSearcher>();
            RegisterWebRepository<PlannerTask>("/Task");
            RegisterWebRepository<Note>("/Note");
            RegisterWebRepository<Blob>("/Blob");
            services.AddSingleton(typeof(ICachedRepositorySource<>), typeof(LocalToRemoteRepositoryBridge<>));
            services.AddSingleton(typeof(ILocalRepository<>), typeof(CachedRepository<>));
        }

        private void RegisterWebRepository<T>(string name) where T:PlannerItemWithDate =>
            services.AddTransient<IDatedRemoteRepository<T>>
                (i => new WebRepository<T>(i.GetRequiredService<IJsonWebService>(), name));
    }
}