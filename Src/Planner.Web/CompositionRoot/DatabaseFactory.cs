using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Planner.Models.Repositories;
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