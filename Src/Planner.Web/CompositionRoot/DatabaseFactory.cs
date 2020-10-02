using Microsoft.Extensions.DependencyInjection;
using Planner.Repository.SqLite;

namespace Planner.Web.CompositionRoot
{
    public static class DatabaseFactory
    {
        public static void ConfigureDatabase(IServiceCollection services)
        {
            var dbCreator = TestDatabaseFactory.TestDatabaseCreator();
            services.AddScoped(i=>dbCreator);
        }
    }
}