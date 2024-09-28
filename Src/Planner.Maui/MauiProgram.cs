using Melville.IOC.AspNet.RegisterFromServiceCollection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Planner.Maui.CompositionRoot;

namespace Planner.Maui;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Configuration.AddUserSecrets<App>();
        builder.ConfigureContainer(new MelvilleServiceProviderFactory(true, 
            (service)=> new IocConfiguration(service, builder.Configuration).Register()));
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "Material");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
