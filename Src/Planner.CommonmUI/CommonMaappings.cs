using System.Text.Json;
using System.Text.Json.Serialization;
using Melville.IOC.IocContainers;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.CommonmUI.RepositoryMapping;
using Planner.Models.HtmlGeneration;
using Planner.Models.Login;
using Planner.Models.Repositories;
using Planner.Models.Time;

namespace Planner.CommonUI;

public readonly struct CommonMaappings(IBindableIocService service)
{
    public void Configure()
    {
        ConfigureClock();
        ConfigureTargets();
        RegisterRepositories();
        SetupJsonSerialization();
        service.Bind<IEventBroadcast<ClearCachesEventArgs>>().
            To<EventBroadcast<ClearCachesEventArgs>>().AsSingleton();
    }

    private void ConfigureTargets()
    {
        service.Bind<IList<TargetSite>>().ToConstant([
            new TargetSite("Planner", "https://planner.drjohnmelville.com"),
            new TargetSite("PlannerLocal", "https://localhost:44370"),
            new TargetSite("LocalFake", ""),
        ]);
    }

    private void ConfigureClock()
    {
        service.Bind<IClock>().ToConstant(SystemClock.Instance);
        service.Bind<IUsersClock>().To<UsersClock>().AsSingleton();
    }

    private void RegisterRepositories()
    {
        service.Bind<IRegisterRepositorySource>().To<RegisterRepositorySource>();
        service.BindGeneric(typeof(ICachedRepositorySource<>),typeof(LocalToRemoteRepositoryBridge<>));
        service.BindGeneric(typeof(ILocalRepository<>), typeof(CachedRepository<>), 
            i=>i.AsSingleton());
    }

    private void SetupJsonSerialization()
    {
        var options = new JsonSerializerOptions();
        options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.IgnoreReadOnlyProperties = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.ReferenceHandler = ReferenceHandler.Preserve;
        service.Bind<JsonSerializerOptions>().ToConstant(options);
    }



}