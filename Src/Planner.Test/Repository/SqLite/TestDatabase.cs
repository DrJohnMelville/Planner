using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Planner.Repository.SqLite;

namespace TUnit.Repository.SqLite;

public class TestDatabase
{
    private readonly DbContextOptions<PlannerDataContext> option;
    public PlannerDataContext NewContext() => new PlannerDataContext(option);

    public TestDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        option = new DbContextOptionsBuilder<PlannerDataContext>()
            .UseSqlite(connection).Options;

        using var context = NewContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}