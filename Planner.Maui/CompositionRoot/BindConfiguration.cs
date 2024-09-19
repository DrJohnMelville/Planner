using Microsoft.Extensions.Configuration;

namespace Planner.Maui.CompositionRoot;

public static class BindConfiguration
{
    public static T Bind<T>(this IConfiguration config) where T: new()
    {
        var ret = new T();
        config.Bind(ret);
        return ret;
    }
}