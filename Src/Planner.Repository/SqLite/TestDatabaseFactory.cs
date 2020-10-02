using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Extensions;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    public static class TestDatabaseFactory
    {
        public static Func<PlannerDataContext> TestDatabaseCreator()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<PlannerDataContext>()
                .UseSqlite(connection).Options;
            Func<PlannerDataContext> ret = ()=>new PlannerDataContext(options);
            SeedDatabase(ret);
            return ret;
        }

        private static void SeedDatabase(Func<PlannerDataContext> contextFactory)
        {
            using var context = contextFactory();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.PlannerTasks.Add(new PlannerTask(Guid.NewGuid())
            {
                Date = SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentDate(),
                Name = "Sample Task"
            });
            context.SaveChanges();
        }
    }
}