using System;
using Melville.MVVM.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Planner.Repository.SqLite;

namespace Planner.Web.CompositionRoot
{
    public static class DatabaseFactory
    {
        public static void ConfigureDatabase(IServiceCollection services)
        {
            RegisterDataDirectory(services);
            RegisterDatabase(services);
        }

        private static void RegisterDatabase(IServiceCollection services)
        {
            services.AddSingleton(i => TestDatabaseFactory.TestDatabaseCreator());
        }

        private static void RegisterDataDirectory(IServiceCollection services)
        {
            services.AddSingleton<IDirectory>(new MemoryDirectory("C:\\Content"));
        }
    }
}