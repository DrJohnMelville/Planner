using Melville.IOC.IocContainers;
using Microsoft.Extensions.Configuration;
using Planner.CommonUI;

namespace Planner.Maui.CompositionRoot;

public readonly struct IocConfiguration(
    IBindableIocService service,
    ConfigurationManager config)
{
    public void Register()
    {
        GC.KeepAlive(config); // Disable the not used warning -- I may use it in the future..
        new CommonMaappings(service).Configure();
        RegistedMauiDefaultPreferences();
    }

    private void RegistedMauiDefaultPreferences()
    {
        service.Bind<ILayoutManagerFactory>().ToConstant(FakeLayoutManagerFactory.Instance);
        service.Bind<IWindowCreator>().ToConstant(FakeWindowCreator.Instance);
    }
}