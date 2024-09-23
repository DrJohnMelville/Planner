using Melville.IOC.IocContainers;
using Microsoft.Extensions.Configuration;
using Planner.Models.Login;

namespace Planner.Maui.CompositionRoot;

public readonly struct IocConfiguration(
    IBindableIocService service, ConfigurationManager config)
{
    public void Register()
    {
            service.Bind<IList<TargetSite>>().ToConstant([
                new TargetSite("Planner", "https://planner.drjohnmelville.com"),
                new TargetSite("PlannerLocal", "https://localhost:44370"),
                new TargetSite("LocalFake", ""),
            ]);
    }
}