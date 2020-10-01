using System;
using Melville.IOC.IocContainers;
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
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var option = new DbContextOptionsBuilder<PlannerDataContext>()
                .UseSqlite(connection).Options;

            SeedDatabase(option);

            services.AddScoped(i=>new PlannerDataContext(option));

        }

        private static void SeedDatabase(DbContextOptions<PlannerDataContext> option)
        {
            using var context = new PlannerDataContext(option);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.PlannerTasks.Add(new RemotePlannerTask(Guid.NewGuid())
            {
                Date = new LocalDate(1975, 07, 28),
                Name = "My Birthday"
            });
            context.SaveChanges();
        }
    }
}