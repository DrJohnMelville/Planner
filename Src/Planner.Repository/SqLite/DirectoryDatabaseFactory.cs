using System;
using Melville.FileSystem;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Planner.Repository.SqLite
{
    public static class DirectoryDatabaseFactory
    {
        public static Func<PlannerDataContext> DatabaseCreator(IDirectory dbDir)
        {
            var ret = DataContextFactory(dbDir.File("database.db"));
            EnsureDatabaseIsLatestVersion(ret);
            return ret;
        }

        private static Func<PlannerDataContext> DataContextFactory(IFile file)
        {
            var connection = new SqliteConnection("DataSource=" + file.Path);
            connection.Open();
            var options = new DbContextOptionsBuilder<PlannerDataContext>()
                .UseSqlite(connection).Options;
            // options must be a local variable -- we capture the options and reuse it in th e lambda
            return () => new PlannerDataContext(options);
        }

        private static void EnsureDatabaseIsLatestVersion(Func<PlannerDataContext> ret) => ret().Database.Migrate();
    }
}