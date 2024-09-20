using Melville.IOC.IocContainers;
using Microsoft.Extensions.Configuration;
using Planner.Models.Login;

namespace Planner.Maui.CompositionRoot;

public readonly struct IocConfiguration(
    IBindableIocService service, ConfigurationManager config)
{
    public void Register()
    {
        service.Bind<IList<TargetSite>>().ToConstant(
            config.GetSection("Sites").Bind<List<TargetSite>>());
    }
}