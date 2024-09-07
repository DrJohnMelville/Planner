using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NodaTime;
using NodaTime.Extensions;
using Planner.Models.Notes;
using Planner.Models.Tasks;

namespace Planner.Repository.SqLite
{
    /// <summary>
    /// Allows the design time EF tools to create a database context
    /// </summary>
    public class DesignTimeFactory : IDesignTimeDbContextFactory<PlannerDataContext>
    {
        public PlannerDataContext CreateDbContext(string[] args) => TestDatabaseFactory.TestDatabaseCreator()();
    }
    public static class TestDatabaseFactory
    {
        public static Func<PlannerDataContext> TestDatabaseCreator()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<PlannerDataContext>()
                .UseSqlite(connection).Options;
            Func<PlannerDataContext> ret = () => new PlannerDataContext(options);
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
                Date = Today(),
                Name = "Sample Task"
            });

            context.Notes.Add(new Note()
            {
                Key = Guid.NewGuid(),
                Date = Today(),
                Title = "Some Text",
                Text = "Try out some **markdown**."
            });
            
            context.SaveChanges();
            
        }

        private static LocalDate Today() => SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentDate();
    }
}